const form = document.getElementById('orderForm');
const statusMessage = document.getElementById('statusMessage');
const ordersTableBody = document.querySelector('#ordersTable tbody');
const submitButton = form.querySelector('button[type="submit"]');
const currencyFormatter = new Intl.NumberFormat('tr-TR', {
  style: 'currency',
  currency: 'TRY'
});

function setStatus(message, type = 'neutral') {
  statusMessage.textContent = message;
  statusMessage.dataset.type = type;
}

function addCell(row, value) {
  const cell = document.createElement('td');
  cell.textContent = value;
  row.appendChild(cell);
}

async function readError(response) {
  try {
    const payload = await response.json();

    if (payload.errors) {
      return Object.values(payload.errors).flat().join(' ');
    }

    return payload.detail || payload.title || `HTTP ${response.status}`;
  } catch {
    return `HTTP ${response.status}`;
  }
}

async function loadOrders() {
  const response = await fetch('/api/orders');

  if (!response.ok) {
    throw new Error(await readError(response));
  }

  const orders = await response.json();
  ordersTableBody.innerHTML = '';

  orders.forEach((order) => {
    const row = document.createElement('tr');
    addCell(row, order.orderNumber);
    addCell(row, order.customerName);
    addCell(row, order.productName);
    addCell(row, order.quantity);
    addCell(row, currencyFormatter.format(order.unitPrice));
    addCell(row, `%${(order.discountRate * 100).toLocaleString('tr-TR')}`);
    addCell(row, currencyFormatter.format(order.totalPrice));
    addCell(row, new Date(order.createdAt).toLocaleString('tr-TR'));
    ordersTableBody.appendChild(row);
  });

  if (orders.length === 0) {
    const row = document.createElement('tr');
    const cell = document.createElement('td');
    cell.colSpan = 8;
    cell.textContent = 'Henüz sipariş bulunmuyor.';
    cell.className = 'empty-state';
    row.appendChild(cell);
    ordersTableBody.appendChild(row);
  }
}

async function createOrder(order) {
  const response = await fetch('/api/orders', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(order)
  });

  if (!response.ok) {
    throw new Error(await readError(response));
  }

  return response.json();
}

form.addEventListener('submit', async (event) => {
  event.preventDefault();

  const order = {
    customerName: document.getElementById('customerName').value.trim(),
    productName: document.getElementById('productName').value.trim(),
    quantity: Number(document.getElementById('quantity').value),
    unitPrice: Number(document.getElementById('unitPrice').value),
    discountRate: Number(document.getElementById('discountRate').value)
  };

  try {
    submitButton.disabled = true;
    setStatus('Sipariş oluşturuluyor...');
    await createOrder(order);
    setStatus('Sipariş başarıyla oluşturuldu.', 'success');
    form.reset();
    document.getElementById('quantity').value = '1';
    document.getElementById('discountRate').value = '0';
    await loadOrders();
  } catch (error) {
    setStatus(`Hata: ${error.message}`, 'error');
  } finally {
    submitButton.disabled = false;
  }
});

loadOrders().catch((error) => {
  setStatus(`Siparişler yüklenemedi: ${error.message}`, 'error');
});
