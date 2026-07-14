# Hata Tespit ve Çözüm Özeti

## 1. Sipariş oluşturma isteği yanlış adrese ve yanlış HTTP metoduyla gönderiliyordu

**Belirti:** Tarayıcının Network sekmesinde `/api/order` adresine `GET` isteği gönderildiği ve siparişin oluşturulamadığı görülüyordu.

**Kök neden:** Backend rotası `/api/orders` ve oluşturma metodu `POST` olmasına rağmen frontend `/api/order` adresine `GET` isteği gönderiyordu. Ayrıca `GET` isteğine gövde eklenmişti.

**Çözüm:** İstek adresi `/api/orders`, HTTP metodu `POST` olarak düzeltildi ve JSON gövdesi bu istekle gönderildi.

## 2. Frontend alan adları backend modeliyle eşleşmiyordu

**Belirti:** Doğru adrese istek gönderildiğinde alanlar varsayılan değerlerle geliyor veya doğrulama hatası oluşuyordu.

**Kök neden:** Frontend `customer_name`, `product_name`, `unit_price` ve `discount_rate` alanlarını gönderirken API modeli camelCase alanlar bekliyordu.

**Çözüm:** İstek modeli `customerName`, `productName`, `quantity`, `unitPrice` ve `discountRate` alanlarıyla gönderilecek şekilde düzeltildi.

## 3. Bildirim servisi için yanlış port kullanılıyordu

**Belirti:** Sipariş oluşmasına rağmen NotificationApi konsolunda bildirim kaydı görünmüyordu.

**Kök neden:** NotificationApi `5049` portunda çalışırken OrderApi yapılandırması `5050` portuna istek gönderiyordu.

**Çözüm:** `NotificationService:BaseUrl` değeri `http://localhost:5049` olarak düzeltildi.

## 4. İndirim hesabı yanlış yapılıyordu

**Belirti:** İndirimli siparişlerin toplam tutarı beklenenden farklı hesaplanıyordu.

**Kök neden:** İndirim tutarı, ara toplam üzerinden oranlanmak yerine `DiscountRate * 100` şeklinde sabit bir tutara dönüştürülüyordu.

**Çözüm:** Toplam tutar `adet × birim fiyat × (1 - indirim oranı)` formülüyle hesaplandı ve sonuç iki ondalık basamağa yuvarlandı.

## 5. DbContext yanlış yaşam süresiyle kaydedilmişti

**Belirti:** Eşzamanlı isteklerde aynı DbContext örneğinin paylaşılması nedeniyle izleme ve thread güvenliği sorunları oluşabilirdi.

**Kök neden:** EF Core `DbContext` thread-safe olmadığı halde `Singleton` olarak kaydedilmişti.

**Çözüm:** `AddDbContext` varsayılan `Scoped` yaşam süresiyle kullanılacak şekilde düzenlendi.

## 6. Bildirim çağrısı beklenmeden başlatılıyor ve hatalar görünmez hale geliyordu

**Belirti:** Sipariş yanıtı döndükten sonra bildirim işlemi tamamlanmadan kesilebiliyor, başarısız HTTP durumları ve bağlantı sorunları teşhis edilemiyordu.

**Kök neden:** Bildirim çağrısı fire-and-forget biçiminde çalıştırılıyor ve tüm hatalar sessizce yutuluyordu.

**Çözüm:** Bildirim çağrısı `await` ile tamamlanıyor. Bildirim servisi yanıt vermese bile sipariş korunuyor, ancak hata ve başarısız durum kodları yapılandırılmış loglara yazılıyor.

## 7. Sipariş numarası bellek içindeki statik sayaçla üretiliyordu

**Belirti:** Uygulama yeniden başladığında sayaç sıfırlanıyor ve eşzamanlı isteklerde aynı numaranın üretilme riski oluşuyordu.

**Kök neden:** Sipariş numarası kalıcı olmayan ve thread-safe olmayan statik bir alanla oluşturuluyordu.

**Çözüm:** Zaman damgası ve GUID parçasından oluşan benzersiz sipariş numarası üretimi eklendi. Veritabanında sipariş numarası için benzersiz indeks tanımlandı.

## 8. Girdi doğrulaması yetersizdi

**Belirti:** Boş metinler, sıfır veya negatif adet, negatif fiyat ve geçersiz indirim oranları gönderilebiliyordu. Null müşteri adı `Trim` sırasında çalışma zamanı hatasına yol açabiliyordu.

**Kök neden:** İstek modelinde doğrulama kuralları ve boşluklardan oluşan metin kontrolü bulunmuyordu.

**Çözüm:** Zorunlu alan, uzunluk ve aralık doğrulamaları eklendi. Müşteri ve ürün adları için boşluk kontrolü yapıldı.

## 9. Oluşturma yanıtındaki kaynak adresi yanlış aksiyona bağlıydı

**Belirti:** `201 Created` yanıtının `Location` başlığı tek bir siparişi göstermeyen listeleme aksiyonuna bağlanıyordu.

**Kök neden:** `CreatedAtAction` çağrısı parametre almayan `GetAll` metodunu hedefliyordu.

**Çözüm:** `GET /api/orders/{id}` uç noktası eklendi ve oluşturma yanıtı bu kaynağa bağlandı.

## 10. Arayüz hata yönetimi ve tablo üretimi güvenli değildi

**Belirti:** API hatalarının ayrıntısı kullanıcıya gösterilmiyor ve kullanıcı girdileri doğrudan `innerHTML` içine yazılıyordu.

**Kök neden:** HTTP hata gövdeleri okunmuyor, tablo satırları şablon metniyle oluşturuluyordu.

**Çözüm:** Problem Details yanıtlarını okuyup gösteren hata yönetimi eklendi. Hücreler `textContent` ile oluşturularak kullanıcı girdilerinin HTML olarak çalışması engellendi.
