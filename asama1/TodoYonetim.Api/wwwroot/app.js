const state = {
    status: "all",
    search: "",
    categoryId: "",
    tagId: "",
    sortBy: "created",
    sortDirection: "desc",
    todos: [],
    categories: [],
    tags: [],
    editingId: null,
    activeTab: "tasks"
};

const elements = {
    todoForm: document.getElementById("todo-form"),
    todoId: document.getElementById("todo-id"),
    title: document.getElementById("title"),
    description: document.getElementById("description"),
    category: document.getElementById("category"),
    dueDate: document.getElementById("due-date"),
    priority: document.getElementById("priority"),
    tagOptions: document.getElementById("tag-options"),
    formTitle: document.getElementById("form-title"),
    saveButton: document.getElementById("save-button"),
    cancelButton: document.getElementById("cancel-button"),
    errorMessage: document.getElementById("error-message"),
    todoList: document.getElementById("todo-list"),
    loading: document.getElementById("loading"),
    emptyState: document.getElementById("empty-state"),
    summary: document.getElementById("summary"),
    search: document.getElementById("search"),
    categoryFilter: document.getElementById("category-filter"),
    tagFilter: document.getElementById("tag-filter"),
    sort: document.getElementById("sort"),
    clearFilters: document.getElementById("clear-filters"),
    categoryForm: document.getElementById("category-form"),
    categoryName: document.getElementById("category-name"),
    categoryList: document.getElementById("category-list"),
    categoryError: document.getElementById("category-error"),
    tagForm: document.getElementById("tag-form"),
    tagName: document.getElementById("tag-name"),
    tagList: document.getElementById("tag-list"),
    tagError: document.getElementById("tag-error"),
    categoryCount: document.getElementById("category-count"),
    tagCount: document.getElementById("tag-count"),
    tasksView: document.getElementById("tasks-view"),
    settingsView: document.getElementById("settings-view"),
    tabButtons: Array.from(document.querySelectorAll("[data-tab]"))
};

async function api(url, options = {}) {
    const response = await fetch(url, {
        headers: { "Content-Type": "application/json", ...(options.headers || {}) },
        ...options
    });

    if (!response.ok) {
        let message = "İşlem tamamlanamadı.";
        try {
            const body = await response.json();
            message = body.detail || body.title || extractValidationMessage(body) || message;
        } catch {
            message = response.statusText || message;
        }
        throw new Error(message);
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

function extractValidationMessage(body) {
    if (!body.errors) {
        return "";
    }
    const messages = Object.values(body.errors).flat();
    return messages[0] || "";
}

async function loadMetadata() {
    const [categories, tags] = await Promise.all([
        api("/api/categories"),
        api("/api/tags")
    ]);
    state.categories = categories;
    state.tags = tags;
    renderMetadata();
}

async function loadTodos() {
    elements.loading.classList.remove("hidden");
    elements.emptyState.classList.add("hidden");
    elements.todoList.innerHTML = "";

    const params = new URLSearchParams({
        status: state.status,
        sortBy: state.sortBy,
        sortDirection: state.sortDirection
    });

    if (state.search) {
        params.set("search", state.search);
    }
    if (state.categoryId) {
        params.set("categoryId", state.categoryId);
    }
    if (state.tagId) {
        params.set("tagId", state.tagId);
    }

    try {
        state.todos = await api(`/api/todos?${params.toString()}`);
        renderTodos();
    } catch (error) {
        elements.emptyState.textContent = error.message;
        elements.emptyState.classList.remove("hidden");
    } finally {
        elements.loading.classList.add("hidden");
    }
}

function renderMetadata() {
    const selectedCategory = elements.category.value;
    const selectedCategoryFilter = elements.categoryFilter.value;
    const selectedTagFilter = elements.tagFilter.value;

    elements.category.innerHTML = '<option value="">Kategori yok</option>' + state.categories
        .map(item => `<option value="${item.id}">${escapeHtml(item.name)}</option>`)
        .join("");
    elements.categoryFilter.innerHTML = '<option value="">Tümü</option>' + state.categories
        .map(item => `<option value="${item.id}">${escapeHtml(item.name)}</option>`)
        .join("");
    elements.tagFilter.innerHTML = '<option value="">Tümü</option>' + state.tags
        .map(item => `<option value="${item.id}">${escapeHtml(item.name)}</option>`)
        .join("");

    elements.category.value = selectedCategory;
    elements.categoryFilter.value = selectedCategoryFilter;
    elements.tagFilter.value = selectedTagFilter;

    elements.tagOptions.innerHTML = state.tags.length
        ? state.tags.map(item => `
            <label class="tag-check">
                <input type="checkbox" name="tagIds" value="${item.id}">
                <span>${escapeHtml(item.name)}</span>
            </label>`).join("")
        : '<span class="muted">Henüz etiket eklenmedi.</span>';

    elements.categoryList.innerHTML = state.categories.length
        ? state.categories.map(item => manageChip(item, "category")).join("")
        : '<span class="muted">Henüz kategori yok.</span>';
    elements.tagList.innerHTML = state.tags.length
        ? state.tags.map(item => manageChip(item, "tag")).join("")
        : '<span class="muted">Henüz etiket yok.</span>';
    elements.categoryCount.textContent = String(state.categories.length);
    elements.tagCount.textContent = String(state.tags.length);
}


function switchTab(tabName) {
    state.activeTab = tabName;
    elements.tasksView.classList.toggle("hidden", tabName !== "tasks");
    elements.settingsView.classList.toggle("hidden", tabName !== "settings");
    elements.tabButtons.forEach(button => {
        const isActive = button.dataset.tab === tabName;
        button.classList.toggle("active", isActive);
        button.setAttribute("aria-selected", String(isActive));
    });
}

function manageChip(item, type) {
    return `<span class="manage-chip">${escapeHtml(item.name)}<button class="chip-delete" type="button" data-delete-${type}="${item.id}" aria-label="Sil">×</button></span>`;
}

function renderTodos() {
    elements.summary.textContent = `${state.todos.length} görev`;
    elements.emptyState.classList.toggle("hidden", state.todos.length > 0);
    elements.todoList.innerHTML = state.todos.map(todoCard).join("");
}

function todoCard(todo) {
    const priorityMap = {
        Dusuk: ["Düşük", "low"],
        Orta: ["Orta", "medium"],
        Yuksek: ["Yüksek", "high"]
    };
    const [priorityText, priorityClass] = priorityMap[todo.priority] || [todo.priority, ""];
    const dueDate = todo.dueDate ? formatDate(todo.dueDate) : "Son tarih yok";
    const category = todo.category ? `<span class="badge category">${escapeHtml(todo.category.name)}</span>` : "";
    const overdue = todo.isOverdue ? '<span class="badge overdue">Gecikmiş</span>' : "";
    const tags = todo.tags.length
        ? `<div class="tags">${todo.tags.map(tag => `<span class="tag-badge">#${escapeHtml(tag.name)}</span>`).join("")}</div>`
        : "";

    return `
        <article class="todo-card ${todo.isCompleted ? "completed" : ""}">
            <input class="toggle" type="checkbox" ${todo.isCompleted ? "checked" : ""} data-toggle="${todo.id}" aria-label="Tamamlanma durumunu değiştir">
            <div>
                <h3 class="todo-title">${escapeHtml(todo.title)}</h3>
                ${todo.description ? `<p class="todo-description">${escapeHtml(todo.description)}</p>` : ""}
                <div class="meta">
                    <span class="badge ${priorityClass}">${priorityText}</span>
                    <span class="badge">${dueDate}</span>
                    ${category}
                    ${overdue}
                </div>
                ${tags}
            </div>
            <div class="card-actions">
                <button class="icon-button" type="button" data-edit="${todo.id}">Düzenle</button>
                <button class="icon-button danger" type="button" data-delete="${todo.id}">Sil</button>
            </div>
        </article>`;
}

function getSelectedTagIds() {
    return Array.from(document.querySelectorAll('input[name="tagIds"]:checked')).map(input => Number(input.value));
}

function setSelectedTagIds(ids) {
    document.querySelectorAll('input[name="tagIds"]').forEach(input => {
        input.checked = ids.includes(Number(input.value));
    });
}

function buildTodoRequest() {
    const dueDate = elements.dueDate.value ? `${elements.dueDate.value}T23:59:59` : null;
    return {
        title: elements.title.value.trim(),
        description: elements.description.value.trim() || null,
        dueDate,
        priority: elements.priority.value,
        categoryId: elements.category.value ? Number(elements.category.value) : null,
        tagIds: getSelectedTagIds()
    };
}

function startEdit(id) {
    const todo = state.todos.find(item => item.id === id);
    if (!todo) {
        return;
    }

    switchTab("tasks");
    state.editingId = id;
    elements.todoId.value = id;
    elements.title.value = todo.title;
    elements.description.value = todo.description || "";
    elements.category.value = todo.category ? String(todo.category.id) : "";
    elements.dueDate.value = todo.dueDate ? todo.dueDate.slice(0, 10) : "";
    elements.priority.value = todo.priority;
    setSelectedTagIds(todo.tags.map(tag => tag.id));
    elements.formTitle.textContent = "Görevi düzenle";
    elements.saveButton.textContent = "Değişiklikleri kaydet";
    elements.cancelButton.classList.remove("hidden");
    elements.title.focus();
    window.scrollTo({ top: elements.todoForm.offsetTop - 30, behavior: "smooth" });
}

function resetForm() {
    state.editingId = null;
    elements.todoForm.reset();
    elements.todoId.value = "";
    elements.priority.value = "Orta";
    elements.formTitle.textContent = "Yeni görev ekle";
    elements.saveButton.textContent = "Görev ekle";
    elements.cancelButton.classList.add("hidden");
    elements.errorMessage.textContent = "";
    elements.dueDate.min = todayValue();
    setSelectedTagIds([]);
}

async function deleteTodo(id) {
    if (!window.confirm("Bu görevi silmek istediğinize emin misiniz?")) {
        return;
    }
    await api(`/api/todos/${id}`, { method: "DELETE" });
    if (state.editingId === id) {
        resetForm();
    }
    await loadTodos();
}

async function toggleTodo(id) {
    await api(`/api/todos/${id}/toggle`, { method: "PATCH" });
    await loadTodos();
}

async function createCategory(name) {
    await api("/api/categories", {
        method: "POST",
        body: JSON.stringify({ name })
    });
    await loadMetadata();
}

async function createTag(name) {
    await api("/api/tags", {
        method: "POST",
        body: JSON.stringify({ name })
    });
    await loadMetadata();
}

async function deleteMetadata(type, id) {
    const label = type === "category" ? "kategoriyi" : "etiketi";
    if (!window.confirm(`Bu ${label} silmek istediğinize emin misiniz?`)) {
        return;
    }
    await api(`/api/${type === "category" ? "categories" : "tags"}/${id}`, { method: "DELETE" });
    await loadMetadata();
    await loadTodos();
}

function formatDate(value) {
    return new Intl.DateTimeFormat("tr-TR", { day: "2-digit", month: "2-digit", year: "numeric" }).format(new Date(value));
}

function todayValue() {
    const now = new Date();
    const local = new Date(now.getTime() - now.getTimezoneOffset() * 60000);
    return local.toISOString().slice(0, 10);
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

let searchTimer;

elements.tabButtons.forEach(button => {
    button.addEventListener("click", () => {
        switchTab(button.dataset.tab);
    });
});

elements.todoForm.addEventListener("submit", async event => {
    event.preventDefault();
    elements.errorMessage.textContent = "";
    const request = buildTodoRequest();

    try {
        if (state.editingId) {
            await api(`/api/todos/${state.editingId}`, { method: "PUT", body: JSON.stringify(request) });
        } else {
            await api("/api/todos", { method: "POST", body: JSON.stringify(request) });
        }
        resetForm();
        await loadTodos();
    } catch (error) {
        elements.errorMessage.textContent = error.message;
    }
});

elements.cancelButton.addEventListener("click", resetForm);

elements.todoList.addEventListener("click", async event => {
    const editButton = event.target.closest("[data-edit]");
    const deleteButton = event.target.closest("[data-delete]");

    try {
        if (editButton) {
            startEdit(Number(editButton.dataset.edit));
        }
        if (deleteButton) {
            await deleteTodo(Number(deleteButton.dataset.delete));
        }
    } catch (error) {
        window.alert(error.message);
    }
});

elements.todoList.addEventListener("change", async event => {
    const toggle = event.target.closest("[data-toggle]");
    if (!toggle) {
        return;
    }
    try {
        await toggleTodo(Number(toggle.dataset.toggle));
    } catch (error) {
        window.alert(error.message);
    }
});

document.querySelectorAll("[data-status]").forEach(button => {
    button.addEventListener("click", async () => {
        document.querySelectorAll("[data-status]").forEach(item => item.classList.remove("active"));
        button.classList.add("active");
        state.status = button.dataset.status;
        await loadTodos();
    });
});

elements.search.addEventListener("input", () => {
    clearTimeout(searchTimer);
    searchTimer = setTimeout(async () => {
        state.search = elements.search.value.trim();
        await loadTodos();
    }, 300);
});

elements.categoryFilter.addEventListener("change", async () => {
    state.categoryId = elements.categoryFilter.value;
    await loadTodos();
});

elements.tagFilter.addEventListener("change", async () => {
    state.tagId = elements.tagFilter.value;
    await loadTodos();
});

elements.sort.addEventListener("change", async () => {
    [state.sortBy, state.sortDirection] = elements.sort.value.split("|");
    await loadTodos();
});

elements.clearFilters.addEventListener("click", async () => {
    state.status = "all";
    state.search = "";
    state.categoryId = "";
    state.tagId = "";
    state.sortBy = "created";
    state.sortDirection = "desc";
    elements.search.value = "";
    elements.categoryFilter.value = "";
    elements.tagFilter.value = "";
    elements.sort.value = "created|desc";
    document.querySelectorAll("[data-status]").forEach(item => item.classList.toggle("active", item.dataset.status === "all"));
    await loadTodos();
});

elements.categoryForm.addEventListener("submit", async event => {
    event.preventDefault();
    elements.categoryError.textContent = "";
    try {
        await createCategory(elements.categoryName.value.trim());
        elements.categoryForm.reset();
    } catch (error) {
        elements.categoryError.textContent = error.message;
    }
});

elements.tagForm.addEventListener("submit", async event => {
    event.preventDefault();
    elements.tagError.textContent = "";
    try {
        await createTag(elements.tagName.value.trim());
        elements.tagForm.reset();
    } catch (error) {
        elements.tagError.textContent = error.message;
    }
});

elements.categoryList.addEventListener("click", async event => {
    const button = event.target.closest("[data-delete-category]");
    if (!button) {
        return;
    }
    elements.categoryError.textContent = "";
    try {
        await deleteMetadata("category", Number(button.dataset.deleteCategory));
    } catch (error) {
        elements.categoryError.textContent = error.message;
    }
});

elements.tagList.addEventListener("click", async event => {
    const button = event.target.closest("[data-delete-tag]");
    if (!button) {
        return;
    }
    elements.tagError.textContent = "";
    try {
        await deleteMetadata("tag", Number(button.dataset.deleteTag));
    } catch (error) {
        elements.tagError.textContent = error.message;
    }
});

async function initialize() {
    elements.dueDate.min = todayValue();
    switchTab("tasks");
    try {
        await loadMetadata();
        await loadTodos();
    } catch (error) {
        elements.loading.classList.add("hidden");
        elements.emptyState.textContent = error.message;
        elements.emptyState.classList.remove("hidden");
    }
}

initialize();
