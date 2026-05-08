document.querySelectorAll('.order__item-header').forEach(item => {
    item.addEventListener('click', (e) => {
        e.preventDefault();
        headElem = item.closest('.order__item');
        bodyElem = headElem.querySelector('.order__item-body');
        btnElem = headElem.querySelector('.order__item-btn')
        btnElem.classList.toggle('order__item-btn--active');
        bodyElem.classList.toggle('order__item-body--active');

    })
})