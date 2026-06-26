"use strict";
document.addEventListener("DOMContentLoaded", () => {
    const emailInput = document.querySelector("#email-input");
    const invalidTooltip = emailInput.parentElement.querySelector(".invalid-tooltip");
    const validTooltip = emailInput.parentElement.querySelector(".valid-tooltip");
    const serverErrorSpan = emailInput.parentElement.querySelector(".field-validation-error");  // asp.net core  filed
    invalidTooltip.textContent = "این فیلد الزامی است. ";
    validTooltip.textContent  = "عالی 👌 ";
    if (serverErrorSpan && serverErrorSpan.textContent.trim() !== "") {
        debugger
        serverErrorSpan.style.visibility = "hidden" ;
        emailInput.setCustomValidity(serverErrorSpan.textContent);
        invalidTooltip.textContent = serverErrorSpan.textContent;
        validTooltip.style.display="none";
        emailInput.classList.remove("is-invalid");
        emailInput.classList.add("is-valid");
    }
    emailInput.addEventListener("input", () => {
        const value = emailInput.value;
        let message = "";

        if (!value) {
            message = "ایمیل الزامی است.";
        } else if (value.length > 50) {
            message = "ایمیل نباید بیشتر از 50 کاراکتر باشد.";
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
            message = "لطفاً یک ایمیل معتبر وارد کنید (مانند example@mail.com)";
        }

        if (message) {
            emailInput.setCustomValidity(message);
            invalidTooltip.textContent = message;
            emailInput.classList.add("is-invalid");
            emailInput.classList.remove("is-valid");
        } else {
            emailInput.setCustomValidity("");
            invalidTooltip.textContent = "";
            validTooltip.textContent = "عالی 👌 ";
            emailInput.classList.remove("is-invalid");
            emailInput.classList.add("is-valid");
        }
    });

  
});
