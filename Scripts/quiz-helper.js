// ============================================
// QUIZ CREATE HELPER FUNCTIONS
// ============================================

// Toast Bildirimleri
function showToast(message, type = 'success') {
    // Toast container yoksa oluştur
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            display: flex;
            flex-direction: column;
            gap: 10px;
        `;
        document.body.appendChild(container);
    }

    // Toast elementi oluştur
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;

    const icons = {
        success: '✓',
        error: '✕',
        warning: '⚠',
        info: 'ℹ'
    };

    const colors = {
        success: '#10b981',
        error: '#ef4444',
        warning: '#f59e0b',
        info: '#3b82f6'
    };

    toast.style.cssText = `
        background: white;
        padding: 16px 20px;
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        display: flex;
        align-items: center;
        gap: 12px;
        min-width: 300px;
        border-left: 4px solid ${colors[type]};
        animation: slideIn 0.3s ease;
    `;

    toast.innerHTML = `
        <span style="
            background: ${colors[type]};
            color: white;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 14px;
        ">${icons[type]}</span>
        <span style="color: #374151; font-weight: 500;">${message}</span>
    `;

    container.appendChild(toast);

    // 3 saniye sonra kaldır
    setTimeout(() => {
        toast.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// CSS animasyonları ekle
if (!document.getElementById('toast-animations')) {
    const style = document.createElement('style');
    style.id = 'toast-animations';
    style.innerHTML = `
        @keyframes slideIn {
            from {
                transform: translateX(400px);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(400px);
                opacity: 0;
            }
        }
    `;
    document.head.appendChild(style);
}

// Loading Spinner
function showLoading(show = true) {
    let spinner = document.getElementById('loading-spinner');

    if (show) {
        if (!spinner) {
            spinner = document.createElement('div');
            spinner.id = 'loading-spinner';
            spinner.style.cssText = `
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0,0,0,0.5);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 99999;
            `;
            spinner.innerHTML = `
                <div style="
                    background: white;
                    padding: 30px;
                    border-radius: 12px;
                    text-align: center;
                ">
                    <div style="
                        border: 4px solid #f3f4f6;
                        border-top: 4px solid #5B0E2D;
                        border-radius: 50%;
                        width: 50px;
                        height: 50px;
                        animation: spin 1s linear infinite;
                        margin: 0 auto 15px;
                    "></div>
                    <p style="margin: 0; color: #374151; font-weight: 500;">Kaydediliyor...</p>
                </div>
            `;
            document.body.appendChild(spinner);

            // Spin animasyonu
            if (!document.getElementById('spinner-animation')) {
                const style = document.createElement('style');
                style.id = 'spinner-animation';
                style.innerHTML = `
                    @keyframes spin {
                        0% { transform: rotate(0deg); }
                        100% { transform: rotate(360deg); }
                    }
                `;
                document.head.appendChild(style);
            }
        }
        spinner.style.display = 'flex';
    } else {
        if (spinner) {
            spinner.style.display = 'none';
        }
    }
}

// API Helper Functions
const QuizAPI = {
    baseUrl: window.location.origin,

    // Quiz Kaydet
    async saveQuiz(quizData) {
        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/save`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(quizData)
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return result;
            } else {
                showToast(result.message, 'error');
                return null;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return null;
        }
    },

    // Soru Kaydet
    async saveQuestion(questionData) {
        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/question/save`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(questionData)
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return result;
            } else {
                showToast(result.message, 'error');
                return null;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return null;
        }
    },

    // Seçenek Kaydet
    async saveOption(optionData) {
        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/option/save`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(optionData)
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return result;
            } else {
                showToast(result.message, 'error');
                return null;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return null;
        }
    },

    // Sonuç Kaydet
    async saveResult(resultData) {
        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/result/save`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(resultData)
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return result;
            } else {
                showToast(result.message, 'error');
                return null;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return null;
        }
    },

    // Soru Sil
    async deleteQuestion(questionId) {
        if (!confirm('Bu soruyu silmek istediğinizden emin misiniz?')) {
            return false;
        }

        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/question/${questionId}`, {
                method: 'DELETE'
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return true;
            } else {
                showToast(result.message, 'error');
                return false;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return false;
        }
    },

    // Seçenek Sil
    async deleteOption(optionId) {
        if (!confirm('Bu seçeneği silmek istediğinizden emin misiniz?')) {
            return false;
        }

        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/option/${optionId}`, {
                method: 'DELETE'
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return true;
            } else {
                showToast(result.message, 'error');
                return false;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return false;
        }
    },

    // Sonuç Sil
    async deleteResult(resultId) {
        if (!confirm('Bu sonucu silmek istediğinizden emin misiniz?')) {
            return false;
        }

        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/result/${resultId}`, {
                method: 'DELETE'
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return true;
            } else {
                showToast(result.message, 'error');
                return false;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return false;
        }
    },

    // Quiz Durumunu Değiştir
    async updateStatus(quizId, status) {
        showLoading(true);
        try {
            const response = await fetch(`${this.baseUrl}/api/quiz/${quizId}/status`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Status: status })
            });
            const result = await response.json();
            showLoading(false);

            if (result.status === 'success') {
                showToast(result.message, 'success');
                return true;
            } else {
                showToast(result.message, 'error');
                return false;
            }
        } catch (error) {
            showLoading(false);
            showToast('Bağlantı hatası: ' + error.message, 'error');
            return false;
        }
    }
};

// Scroll pozisyonunu kaydet
function saveScrollPosition() {
    sessionStorage.setItem('quizCreateScroll', window.scrollY);
}

// Scroll pozisyonunu geri yükle
function restoreScrollPosition() {
    const scrollPos = sessionStorage.getItem('quizCreateScroll');
    if (scrollPos) {
        window.scrollTo(0, parseInt(scrollPos));
        sessionStorage.removeItem('quizCreateScroll');
    }
}

// Sayfa yüklendiğinde scroll pozisyonunu geri yükle
window.addEventListener('load', restoreScrollPosition);