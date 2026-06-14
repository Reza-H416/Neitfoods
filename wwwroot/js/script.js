// NutShop JavaScript

document.addEventListener("DOMContentLoaded", function() {
    console.log("NeitFoods loaded");
    initializeEventListeners();
});

function initializeEventListeners() {
    const addToCartForms = document.querySelectorAll(".add-to-cart-form");
    addToCartForms.forEach(form => {
        form.addEventListener("submit", function(e) {
            e.preventDefault();
            console.log("Adding to cart...");
            this.submit();
        });
    });
}

function updateCart() {
    console.log("Cart updated");
    location.reload();
}