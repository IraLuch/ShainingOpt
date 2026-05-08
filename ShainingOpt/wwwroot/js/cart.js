document.addEventListener('click', async function (e) {
    const btnElem = e.target.closest('[data-action]');
    console.log(btnElem)
    if (!btnElem) return;

    e.preventDefault();

    const cartItem = btnElem.closest('.cart__item')
    if (!cartItem) return;
    const qntElem = cartItem.querySelector('.cart__quantity-value');
    const cartItemId = cartItem.querySelector('.cart-item-id').value;
    const priceElem = cartItem.querySelector('.price');

    const totalSumElem = document.getElementById('totalSum');
    const finalSumElem = document.getElementById('sum');

    if (!qntElem || !priceElem || !totalSumElem || !finalSumElem) return;


    const min = +qntElem.getAttribute('min');
    const max = +qntElem.getAttribute('max');
    let count = parseFloat(qntElem.textContent);

    let totalSum = parseFloat(totalSumElem.textContent);
    const price = parseFloat(priceElem.value);
    let finalSum = parseFloat(finalSumElem.textContent);



    if (btnElem.dataset.action === 'plus' && count < max) {
        count++;
        totalSum += price;
        finalSum += price;

    const response = await fetch('/Cart/UpdateCartItem', {
        method: 'POST', headers: {
            'Content-Type': 'application/json',
            "RequestVerificationToken": document.querySelector("input[name='__RequestVerificationToken']").value
        },
        body: JSON.stringify({
            CartItemId: +cartItemId,
            Quantity: 1
        })
    })
        const data = await response.json();
        if (!data.success) {
            console.error("Ошибка обновления");
        }


    }

    if (btnElem.dataset.action === 'minus' && count > min) {
        count--;
        totalSum -= price;
        finalSum -= price;

        const response = await fetch('/Cart/UpdateCartItem', {
            method: 'POST', headers: {
                'Content-Type': 'application/json',
                "RequestVerificationToken": document.querySelector("input[name='__RequestVerificationToken']").value
            },
            body: JSON.stringify({
                CartItemId: +cartItemId,
                Quantity: -1
            })
        })
        const data = await response.json();
        if (!data.success) {
            console.error("Ошибка обновления");
        }

    }
   
    qntElem.textContent = count;
    totalSumElem.textContent = totalSum;
    finalSumElem.textContent = finalSum;


  
})