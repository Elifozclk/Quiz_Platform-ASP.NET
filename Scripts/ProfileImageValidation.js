// ========================================
// PROFIL RESMİ CLIENT-SIDE VALIDATION
// Register.aspx ve Settings.aspx için
// ========================================

// Kurallar
const MAX_FILE_SIZE_MB = 5;
const MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024;
const MIN_IMAGE_WIDTH = 200;
const MIN_IMAGE_HEIGHT = 200;
const MAX_IMAGE_WIDTH = 2000;
const MAX_IMAGE_HEIGHT = 2000;
const ALLOWED_EXTENSIONS = ['jpg', 'jpeg', 'png'];

/**
 * Profil resmini doğrular (Client-side)
 * @param {FileInput} fileInput - File input element
 * @param {Function} callback - Callback fonksiyonu (isValid, errorMessage)
 */
function validateProfileImage(fileInput, callback) {
    // Dosya seçilmiş mi?
    if (!fileInput.files || fileInput.files.length === 0) {
        callback(false, "❌ Lütfen bir dosya seçin.");
        return;
    }

    const file = fileInput.files[0];

    // 1. Dosya boyutu kontrolü
    if (file.size > MAX_FILE_SIZE_BYTES) {
        const sizeMB = (file.size / 1024 / 1024).toFixed(2);
        callback(false, `❌ Dosya boyutu çok büyük. Maksimum ${MAX_FILE_SIZE_MB} MB yükleyebilirsiniz. (Yüklenen: ${sizeMB} MB)`);
        return;
    }

    if (file.size === 0) {
        callback(false, "❌ Dosya boş olamaz.");
        return;
    }

    // 2. Dosya uzantısı kontrolü
    const fileName = file.name.toLowerCase();
    const extension = fileName.split('.').pop();

    if (!ALLOWED_EXTENSIONS.includes(extension)) {
        callback(false, `❌ Geçersiz dosya formatı. Sadece ${ALLOWED_EXTENSIONS.join(', ').toUpperCase()} dosyaları yüklenebilir.`);
        return;
    }

    // 3. MIME type kontrolü (ekstra güvenlik)
    if (!file.type.startsWith('image/')) {
        callback(false, "❌ Geçersiz dosya tipi. Sadece görsel dosyaları yüklenebilir.");
        return;
    }

    // 4. Görsel boyutu kontrolü (Image olarak yükle)
    const reader = new FileReader();
    reader.onload = function (e) {
        const img = new Image();
        img.onload = function () {
            const width = img.width;
            const height = img.height;

            // Minimum boyut kontrolü
            if (width < MIN_IMAGE_WIDTH || height < MIN_IMAGE_HEIGHT) {
                callback(false, `❌ Görsel boyutu çok küçük. Minimum ${MIN_IMAGE_WIDTH}x${MIN_IMAGE_HEIGHT}px olmalı. (Yüklenen: ${width}x${height}px)`);
                return;
            }

            // Maksimum boyut kontrolü
            if (width > MAX_IMAGE_WIDTH || height > MAX_IMAGE_HEIGHT) {
                callback(false, `❌ Görsel boyutu çok büyük. Maksimum ${MAX_IMAGE_WIDTH}x${MAX_IMAGE_HEIGHT}px olmalı. (Yüklenen: ${width}x${height}px)`);
                return;
            }

            // Aspect ratio kontrolü (toleranslı)
            const aspectRatio = width / height;
            if (aspectRatio < 0.75 || aspectRatio > 1.33) {
                callback(false, `❌ Görsel oranı uygun değil. Tercihen kare (1:1) görsel yükleyin. (Yüklenen: ${width}x${height}px, Oran: ${aspectRatio.toFixed(2)})`);
                return;
            }

            // ✅ Tüm kontroller başarılı
            callback(true, `✅ Geçerli görsel (${width}x${height}px, ${(file.size / 1024).toFixed(0)} KB)`);
        };

        img.onerror = function () {
            callback(false, "❌ Görsel yüklenemedi. Dosya bozuk olabilir.");
        };

        img.src = e.target.result;
    };

    reader.onerror = function () {
        callback(false, "❌ Dosya okunamadı.");
    };

    reader.readAsDataURL(file);
}

/**
 * Görsel önizlemesi göster
 * @param {FileInput} input - File input element
 * @param {Image} imgPreview - Önizleme img elementi
 */
function previewImage(input, imgPreview) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            imgPreview.src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// ========================================
// REGISTER.ASPX İÇİN EVENT LISTENER
// ========================================
/*
Register.aspx'e şu kodu ekleyin:

<script>
    document.addEventListener('DOMContentLoaded', function() {
        const fileInput = document.getElementById('<%= fuProfileImage.ClientID %>');
        const imgPreview = document.getElementById('imgProfilePreview'); // Önizleme için img elementi
        const validationMessage = document.getElementById('profileImageValidation');

        if (fileInput) {
            fileInput.addEventListener('change', function() {
                // Önizleme göster (opsiyonel)
                if (imgPreview) {
                    previewImage(fileInput, imgPreview);
                }

                // Validasyon yap
                validateProfileImage(fileInput, function(isValid, message) {
                    if (validationMessage) {
                        validationMessage.innerHTML = message;
                        validationMessage.className = isValid ? 'alert alert-success' : 'alert alert-danger';
                        validationMessage.style.display = 'block';
                    }

                    // Form submit butonunu devre dışı bırak/aktif et
                    const submitBtn = document.getElementById('<%= btnRegister.ClientID %>');
                    if (submitBtn) {
                        submitBtn.disabled = !isValid;
                    }
                });
            });
        }
    });
</script>
*/

// ========================================
// SETTINGS.ASPX İÇİN EVENT LISTENER
// ========================================
/*
Settings.aspx'e şu kodu ekleyin:

<script>
    document.addEventListener('DOMContentLoaded', function() {
        const fileInput = document.getElementById('<%= fuProfileImage.ClientID %>');
        const imgPreview = document.getElementById('<%= imgProfile.ClientID %>');
        const uploadBtn = document.getElementById('<%= btnUploadImage.ClientID %>');
        const validationMessage = document.getElementById('profileImageValidation');

        if (fileInput) {
            fileInput.addEventListener('change', function() {
                // Önizleme göster
                if (imgPreview) {
                    previewImage(fileInput, imgPreview);
                }

                // Validasyon yap
                validateProfileImage(fileInput, function(isValid, message) {
                    if (validationMessage) {
                        validationMessage.innerHTML = message;
                        validationMessage.className = isValid ? 'alert alert-success' : 'alert alert-danger';
                        validationMessage.style.display = 'block';
                    }

                    // Upload butonunu devre dışı bırak/aktif et
                    if (uploadBtn) {
                        uploadBtn.disabled = !isValid;
                    }
                });
            });
        }
    });
</script>
*/

// ========================================
// HTML ÖRNEĞİ - REGISTER.ASPX
// ========================================
/*
<div class="form-group">
    <label>Profil Resmi</label>
    <asp:FileUpload ID="fuProfileImage" runat="server" CssClass="form-control" accept="image/*" />
   
    <!-- Önizleme -->
    <div class="mt-2">
        <img id="imgProfilePreview" src="~/Uploads/ProfileImages/default-user.png"
             alt="Profil Önizleme" style="max-width: 200px; border-radius: 50%;" />
    </div>
   
    <!-- Validasyon mesajı -->
    <div id="profileImageValidation" class="mt-2" style="display:none;"></div>
   
    <!-- Kurallar -->
    <small class="form-text text-muted">
        <strong>Kurallar:</strong><br/>
        • Maksimum dosya boyutu: 5 MB<br/>
        • Desteklenen formatlar: JPG, JPEG, PNG<br/>
        • Minimum boyut: 200x200px<br/>
        • Maksimum boyut: 2000x2000px<br/>
        • Tercihen kare (1:1) görsel
    </small>
</div>
*/

// ========================================
// HTML ÖRNEĞİ - SETTINGS.ASPX
// ========================================
/*
<div class="form-group">
    <label>Profil Resmi</label>
    
    <!-- Mevcut resim -->
    <div class="mb-2">
        <asp:Image ID="imgProfile" runat="server" 
                   CssClass="img-thumbnail" 
                   style="max-width: 200px; border-radius: 50%;" />
    </div>
    
    <!-- Yeni resim seç -->
    <asp:FileUpload ID="fuProfileImage" runat="server" CssClass="form-control mb-2" accept="image/*" />
    
    <!-- Validasyon mesajı -->
    <div id="profileImageValidation" class="mb-2" style="display:none;"></div>
    
    <!-- Butonlar -->
    <div class="btn-group">
        <asp:Button ID="btnUploadImage" runat="server" 
                    Text="Resmi Yükle" 
                    CssClass="btn btn-primary" 
                    OnClick="btnUploadImage_Click" />
        
        <asp:Button ID="btnUseDefaultProfileImage" runat="server" 
                    Text="Varsayılan Resmi Kullan" 
                    CssClass="btn btn-secondary" 
                    OnClick="btnUseDefaultProfileImage_Click" />
    </div>
    
    <!-- Kurallar -->
    <small class="form-text text-muted mt-2">
        <strong>Kurallar:</strong> Max 5 MB, JPG/PNG, 200x200px - 2000x2000px, Tercihen kare
    </small>
</div>
*/
