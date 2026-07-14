# Görev Yönetim Uygulaması

Proje .NET 8, ASP.NET Core Web API, Entity Framework Core, SQLite ve sade HTML/CSS/JavaScript kullanılarak hazırlanmıştır. Çözüm; ana görev API'si, görev oluşturma kayıtlarını tutan ikinci bir mini servis ve birim testi projesinden oluşur.

## Projeler

- `TodoYonetim.Api`: Görev, kategori ve etiket yönetimi ile web arayüzünü içerir.
- `TodoLog.Api`: Yeni görev oluşturulduğunda gönderilen kayıtları SQLite veritabanında saklar.
- `TodoYonetim.Tests`: Görev servisinin oluşturma, doğrulama, tamamlama ve silme davranışlarını test eder.

## Visual Studio ile çalıştırma

1. `TodoYonetimUygulamasi.sln` dosyasını Visual Studio ile açın.
2. Çözüme sağ tıklayıp `Configure Startup Projects` bölümünü açın.
3. `Multiple startup projects` seçeneğini seçin.
4. `TodoLog.Api` ve `TodoYonetim.Api` projelerini `Start` olarak ayarlayın.
5. `F5` veya `Ctrl+F5` ile çalıştırın.
6. Ana uygulama `http://localhost:5050`, log servisi `http://localhost:5051` adresinde açılır.

Windows üzerinde çözüm klasöründeki `start.bat` dosyası da iki servisi ayrı terminallerde çalıştırır.

## Terminal ile çalıştırma

İlk terminal:

```bash
cd TodoLog.Api
dotnet restore
dotnet run
```

İkinci terminal:

```bash
cd TodoYonetim.Api
dotnet restore
dotnet run
```

Testler:

```bash
dotnet test TodoYonetimUygulamasi.sln
```

## Zorunlu gereksinimler

- Görev ekleme, listeleme, güncelleme, silme
- Tamamlandı olarak işaretleme ve tamamlanmadı durumuna geri alma
- Başlık, açıklama, son tarih ve düşük/orta/yüksek öncelik
- Tümü, tamamlanan ve tamamlanmayan filtreleri
- Geçmiş tarihli yeni görev oluşturmayı engelleme
- Süresi geçmiş tamamlanmamış görevleri gecikmiş etiketiyle gösterme
- .NET 8 Web API
- SQLite ve Entity Framework Core
- Controller, Service, Repository ve DbContext katmanları
- Uygun HTTP durum kodları
- Çalışan web arayüzü

## Bonus özellikler

- Ayrı mini log mikroservisi
- Kategori ekleme, listeleme ve silme
- Etiket ekleme, listeleme ve silme
- Bir göreve birden fazla etiket bağlama
- Başlık, açıklama, kategori ve etikette arama
- Durum, kategori ve etikete göre filtreleme
- Tarih, son tarih, başlık, öncelik, kategori ve duruma göre sıralama
- Birim testleri
- Kategori ve etiketlerin ayrı Ayarlar sekmesinden yönetimi

Veritabanları ilk çalıştırmada otomatik oluşturulur. Ana API log servisine erişemezse görev işlemi yine tamamlanır; görev verileri `todo-management.db`, yeni görev kayıtları `todo-logs.db` içinde tutulur.
