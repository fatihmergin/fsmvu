# FatihMehmetErgin

Bu repository, iki farklı .NET 8 tabanlı proje çalışmasını içermektedir.

## 📂 Aşama 1 – Görev Yönetim Uygulaması

Bu projede;

- .NET 8, ASP.NET Core Web API, Entity Framework Core ve SQLite kullanılarak Görev Yönetim Sistemi geliştirildi.
- Görev ekleme, güncelleme, silme ve listeleme işlemleri gerçekleştirildi.
- Kategori ve etiket yönetimi eklendi.
- Durum, kategori, etiket ve arama filtreleri oluşturuldu.
- Ayrı bir log mikroservisi (TodoLog.Api) ile yeni görev kayıtları tutuldu.
- Birim testleri yazıldı.

## 📂 Aşama 2 – Sipariş Yönetim Sistemi (Hata Giderme)

Bu projede;

- OrderApi ve NotificationApi olmak üzere iki mikroservis üzerinde çalışıldı.
- Sipariş oluşturma ve listeleme işlemleri geliştirildi.
- SQLite ile veri kalıcılığı sağlandı.
- Mikroservisler arasındaki bildirim akışı düzenlendi.
- Sistemde bulunan hatalar analiz edilerek giderildi.
- Tespit edilen hataların nedenleri ve çözümleri **HATA_OZETI.md** dosyasında dokümante edildi.

## 📦 Proje Dosyaları

Repository içerisinde **FatihMehmetErgin.zip** dosyası bulunmaktadır. Bu dosya içerisinde **Aşama 1** ve **Aşama 2** projeleri ayrı ZIP dosyaları halinde yer almaktadır.

---

**Teknolojiler:** .NET 8, ASP.NET Core Web API, Entity Framework Core, SQLite, HTML, CSS, JavaScript.