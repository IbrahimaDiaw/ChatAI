window.scrollToBottom = () => {
    const chatContent = document.querySelector('.e-assistview-content');
    if (chatContent) {
        chatContent.scrollTop = chatContent.scrollHeight;
    }
};

// Auto-focus on message input
window.focusMessageInput = () => {
    const messageInput = document.querySelector('.e-assistview-footer input');
    if (messageInput) {
        messageInput.focus();
    }
};

// Copy text to clipboard
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch (err) {
        console.error('Failed to copy text: ', err);
        return false;
    }
};

// Show browser notification
window.showBrowserNotification = (title, body) => {
    if ("Notification" in window) {
        if (Notification.permission === "granted") {
            new Notification(title, { body });
        } else if (Notification.permission !== "denied") {
            Notification.requestPermission().then(permission => {
                if (permission === "granted") {
                    new Notification(title, { body });
                }
            });
        }
    }
};

// Initialize app
window.initializeApp = () => {
    // Request notification permission
    if ("Notification" in window && Notification.permission === "default") {
        Notification.requestPermission();
    }

    // Focus on message input when page loads
    setTimeout(() => {
        focusMessageInput();
    }, 1000);
};

// Call initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', initializeApp);