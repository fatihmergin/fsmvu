# Sipariş Yönetim Sistemi — Hata Giderme Senaryosu

Bu çözüm iki .NET 8 mikroservisinden oluşur:

- **OrderApi**: `http://localhost:5080`
- **NotificationApi**: `http://localhost:5049`

OrderApi sipariş oluşturma ve listeleme işlemlerini yürütür, SQLite veritabanını kullanır ve `wwwroot` altındaki web arayüzünü sunar. Sipariş başarıyla kaydedildikten sonra NotificationApi çağrılır ve bildirim bilgisi servis konsoluna yazılır.

## Çalıştırma

### Terminal 1

```bash
cd src/NotificationApi
dotnet restore
dotnet run
```

### Terminal 2

```bash
cd src/OrderApi
dotnet restore
dotnet run
```

Tarayıcıdan aşağıdaki adresi açın:

```text
http://localhost:5080
```

## Kontrol Akışı

1. NotificationApi konsolunda `http://localhost:5049` adresinin dinlendiğini doğrulayın.
2. OrderApi konsolunda `http://localhost:5080` adresinin dinlendiğini doğrulayın.
3. Web formundan müşteri, ürün, adet, birim fiyat ve indirim oranı girin.
4. Tarayıcının Network sekmesinde `POST /api/orders` isteğinin `201 Created` döndürdüğünü kontrol edin.
5. Siparişin arayüzde listelendiğini ve NotificationApi konsolunda bildirim kaydının oluştuğunu doğrulayın.
6. OrderApi kapatılıp açıldıktan sonra daha önce oluşturulan siparişlerin SQLite veritabanından tekrar listelendiğini kontrol edin.

## API Uç Noktaları

| Servis | Metot | Adres | Açıklama |
|---|---|---|---|
| OrderApi | GET | `/api/orders` | Siparişleri listeler |
| OrderApi | GET | `/api/orders/{id}` | Tek siparişi getirir |
| OrderApi | POST | `/api/orders` | Yeni sipariş oluşturur |
| NotificationApi | POST | `/api/notify` | Bildirim kaydını konsola yazar |

## Örnek Sipariş İsteği

```json
{
  "customerName": "Ayşe Yılmaz",
  "productName": "Klavye",
  "quantity": 2,
  "unitPrice": 750,
  "discountRate": 0.10
}
```

Bu istek için ara toplam `1500 TL`, yüzde 10 indirim sonrasındaki toplam ise `1350 TL` olur.

Bulunan hataların belirtileri, kök nedenleri ve kalıcı çözümleri `HATA_OZETI.md` dosyasında açıklanmıştır.
