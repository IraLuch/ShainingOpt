document.addEventListener('click', (e) => {
    const btnElem = e.target.closest('[data-action]');
    console.log(btnElem)
    if (!btnElem) return;

    e.preventDefault();
    const qntElem = document.querySelector('.product__quantity-value');

    const min = +qntElem.getAttribute('min');
    const max = +qntElem.getAttribute('max');
    let count = parseInt(qntElem.textContent);

    if (btnElem.dataset.action === 'plus' && count < max) {
        count++;

    }

    if (btnElem.dataset.action === 'minus' && count > min) {
        count--;

    }
        qntElem.textContent = count;

    if (btnElem.dataset.action === 'size') {
        document.querySelectorAll('[data-action="size"]').forEach((item) => {
            item.classList.remove('product__variant--active');
        })
        btnElem.classList.add('product__variant--active');
    }


    //if (btnElem.dataset.action === 'color') {
    //    document.querySelectorAll('[data-action="color"]').forEach((item) => {
    //        item.classList.remove('product__variant--active');
    //    })
    //    btnElem.classList.add('product__variant--active');
    //}
})

