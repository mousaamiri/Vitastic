const CartManager = {

  // #region API Response Handler
  /**
   * Handle API response and parse JSON
   * @param {Response} response - Fetch API response object
   * @returns {Object|null} Parsed result or null on error
   */
  async handleApiResponse(response) {
    if (response.status === 401) {
      this.showNotification('لطفاً وارد حساب کاربری خود شوید.', 'error');
      return null;
    }

    const text = await response.text();
    if (!text.trim()) {
      this.showNotification('پاسخ نامعتبر از سرور.', 'error');
      return null;
    }

    try {
      const result = JSON.parse(text);
      if (!result.isSuccess) {
        this.showNotification(result.message || 'خطایی رخ داد', 'error');
        return null;
      }
      return result;
    } catch {
      this.showNotification('خطا در پردازش پاسخ سرور.', 'error');
      return null;
    }
  },
  // #endregion

  // #region Add to Cart
  /**
   * Add course to shopping cart
   * @param {string} courseId - Course GUID
   * @param {HTMLElement} button - Button element that triggered the action
   */
  async addToCart(courseId, button) {
    const originalText = button.innerHTML;
    const originalClasses = button.className;

    // Show loading state
    button.disabled = true;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> در حال افزودن...';

    try {
      const response = await fetch('/Cart/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(courseId)
      });

      const result = await this.handleApiResponse(response);

      if (!result) {
        // Restore original state on error
        button.innerHTML = originalText;
        button.disabled = false;
        return;
      }

      // Show success notification
      this.showNotification('دوره با موفقیت به سبد خرید اضافه شد', 'success');

      // Update cart count badge
      this.updateCartCount();

      // Change button to "in cart" state
      button.className = 'btn-course btn btn-secondary w-100 mt-2';
      button.innerHTML = '<i class="fas fa-check-circle me-1"></i> در سبد خرید شما';
      button.disabled = true;

    } catch (error) {
      console.error('خطا در افزودن به سبد:', error);
      this.showNotification('خطا در ارتباط با سرور', 'error');

      // Restore original state
      button.innerHTML = originalText;
      button.className = originalClasses;
      button.disabled = false;
    }
  },
  // #endregion

  // #region Remove from Cart
  /**
   * Remove item from shopping cart
   * @param {string} cartItemId - Cart item GUID
   */
  async removeFromCart(cartItemId) {
    if (!confirm('آیا از حذف این دوره از سبد خرید اطمینان دارید؟')) return;

    try {
      const response = await fetch(`/Cart/remove/${cartItemId}`, {
        method: 'DELETE'
      });

      const result = await this.handleApiResponse(response);
      if (!result) return;

      this.showNotification('دوره از سبد خرید حذف شد', 'success');
      this.updateCartCount();

      // Reload page after short delay
      setTimeout(() => location.reload(), 1000);

    } catch (error) {
      console.error('خطا در حذف از سبد:', error);
      this.showNotification('خطا در ارتباط با سرور', 'error');
    }
  },
  // #endregion

  // #region Clear Cart
  /**
   * Clear all items from shopping cart
   */
  async clearCart() {
    if (!confirm('آیا از خالی کردن سبد خرید اطمینان دارید؟')) return;

    try {
      const response = await fetch('/Cart/clear', { method: 'DELETE' });

      const result = await this.handleApiResponse(response);
      if (!result) return;

      this.showNotification('سبد خرید خالی شد', 'success');
      this.updateCartCount();

      // Reload page after short delay
      setTimeout(() => location.reload(), 1000);

    } catch (error) {
      console.error('خطا در خالی کردن سبد:', error);
      this.showNotification('خطا در ارتباط با سرور', 'error');
    }
  },
  // #endregion

  // #region Checkout
  /**
   * Process checkout and redirect to orders page
   */
  async checkout() {
    try {
      const response = await fetch('/Cart/checkout', { method: 'POST' });

      const result = await this.handleApiResponse(response);
      if (!result) return;

      this.showNotification('پرداخت با موفقیت انجام شد', 'success');

      // Redirect to orders page
      setTimeout(() => window.location.href = '/Orders', 1500);

    } catch (error) {
      console.error('خطا در پرداخت:', error);
      this.showNotification('خطا در ارتباط با سرور', 'error');
    }
  },
  // #endregion

  // #region Update Cart Count Badge
  /**
   * Fetch and update cart item count in header badge
   */
  async updateCartCount() {
    try {
      const response = await fetch('/Cart/count');
      if (!response.ok) return;

      const text = await response.text();
      if (!text) return;

      const result = JSON.parse(text);
      const badge = document.querySelector('.cart-count-badge');
      if (!badge) return;

      const count = result.data || 0;
      badge.textContent = count;
      badge.style.display = count > 0 ? 'inline-block' : 'none';

    } catch (error) {
      console.error('خطا در بروزرسانی تعداد سبد:', error);
    }
  },
  // #endregion

  // #region Notification System
  /**
   * Show toast notification
   * @param {string} message - Message to display
   * @param {string} type - Notification type: 'success', 'error', 'info'
   */
  showNotification(message, type = 'info') {
    // Remove existing notification if any
    const existing = document.querySelector('.cart-notification');
    if (existing) existing.remove();

    // Icon mapping
    const icons = {
      success: 'fas fa-check-circle',
      error: 'fas fa-exclamation-circle',
      info: 'fas fa-info-circle'
    };

    // Create notification element
    const notification = document.createElement('div');
    notification.className = `cart-notification cart-notification-${type}`;
    notification.innerHTML = `
      <i class="${icons[type] || icons.info}"></i>
      <span>${message}</span>
    `;

    document.body.appendChild(notification);

    // Trigger animation
    requestAnimationFrame(() => notification.classList.add('show'));

    // Auto remove after 3 seconds
    setTimeout(() => {
      notification.classList.remove('show');
      setTimeout(() => notification.remove(), 300);
    }, 3000);
  },
  // #endregion

  // #region Initialization
  /**
   * Initialize cart manager and bind event listeners
   */
  init() {
    // Delegate: Add to cart buttons
    document.addEventListener('click', (e) => {
      const btn = e.target.closest('.add-to-cart-btn');
      if (btn) {
        e.preventDefault();
        const courseId = btn.dataset.courseId;
        if (courseId) this.addToCart(courseId, btn);
      }
    });

    // Delegate: Remove from cart buttons
    document.addEventListener('click', (e) => {
      const btn = e.target.closest('.remove-from-cart-btn');
      if (btn) {
        e.preventDefault();
        const itemId = btn.dataset.itemId;
        if (itemId) this.removeFromCart(itemId);
      }
    });

    // Delegate: Clear cart button
    document.addEventListener('click', (e) => {
      if (e.target.closest('.clear-cart-btn')) {
        e.preventDefault();
        this.clearCart();
      }
    });

    // Delegate: Checkout button
    document.addEventListener('click', (e) => {
      if (e.target.closest('.checkout-btn')) {
        e.preventDefault();
        this.checkout();
      }
    });

    // Update cart count on page load
    this.updateCartCount();
  }
  // #endregion
};

// #region Auto-Initialize
// Initialize cart manager when DOM is ready
document.addEventListener('DOMContentLoaded', () => CartManager.init());
// #endregion
