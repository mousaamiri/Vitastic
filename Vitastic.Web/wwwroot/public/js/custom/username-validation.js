"use strict";
document.addEventListener("DOMContentLoaded", () => {
    const usernameInput = document.querySelector("#username");
    const invalidTooltip = usernameInput.parentElement.querySelector(".invalid-tooltip");
    const validTooltip = usernameInput.parentElement.querySelector(".valid-tooltip");
    const serverErrorSpan = usernameInput.parentElement.querySelector(".field-validation-error"); // asp.net core  filed
    invalidTooltip.textContent = "نام کاربری الزامی است. ";
    validTooltip.textContent = "عالی 👌";
    debugger
    if (serverErrorSpan && serverErrorSpan.textContent.trim() !== "") {
        debugger
        serverErrorSpan.style.display = "none" ;
        invalidTooltip.textContent = serverErrorSpan.textContent;
        validTooltip.style.display ="none";
        usernameInput.setCustomValidity(serverErrorSpan.textContent);
        usernameInput.classList.add("is-invalid");
        usernameInput.classList.remove("is-valid");
        return false;
    }
    usernameInput.addEventListener("input", () => {
        const value = usernameInput.value.trim();
        let message = "";

        if (!value) {
            message = "نام کاربری الزامی است.";
        } else if (value.length < 4) {
            message = "نام کاربری باید حداقل ۴ کاراکتر باشد.";
        } else if (value.length > 20) {
            message = "نام کاربری نباید بیشتر از ۲۰ کاراکتر باشد.";
        } else if (!/^[a-zA-Z0-9_]+$/.test(value)) {
            message = "فقط حروف انگلیسی، اعداد و _ مجاز هستند.";
        }

        if (message) {
            usernameInput.setCustomValidity(message);
            invalidTooltip.textContent = message;
            usernameInput.classList.add("is-invalid");
            usernameInput.classList.remove("is-valid");
        } else {
            usernameInput.setCustomValidity("");
            invalidTooltip.textContent = "";
            validTooltip.textContent = "عالی 👌";
            usernameInput.classList.remove("is-invalid");
            usernameInput.classList.add("is-valid");
        }
    });


});
