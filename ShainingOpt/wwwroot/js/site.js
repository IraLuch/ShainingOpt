function openPopup(){
    const popupElem = document.querySelector('.popup');
    popupElem.classList.add('popup--active');
}

document.querySelector('.header__login-btn').addEventListener('click', () => {
  
    openPopup();
})

document.querySelector('.popup__close').addEventListener('click', () => {
    console.log('Close');
    const popupElem = document.querySelector('.popup');
    popupElem.classList.remove('popup--active');
})

document.querySelector('#loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const data = new FormData(e.target);

    const response = await fetch('/Account/Login', {
        method: "POST",
        body: data
    }); 
    const result = await response.json();

    if (result.success) {
        window.location.href = '/Account/Profile';
    }
    else {
        const erElem = document.querySelector('.form__error')
        erElem.innerText = result.message;
        erElem.style.display = "block";
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const returnUrl = window.location.search;
    if (returnUrl.includes("ReturnUrl")) {
        openPopup();
    }
})