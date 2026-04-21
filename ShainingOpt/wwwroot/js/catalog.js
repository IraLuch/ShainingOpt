document.querySelectorAll('.filter__title').forEach(title => {
    title.addEventListener('click', function () {
        const el = document.querySelector('.filter__groups');
        const style = getComputedStyle(el);
        if (style.flexDirection === 'row') {
            return;
        }
        const group = this.closest('.filter__group');
        const items = group.querySelector('.filter__items');
        console.log('TITLE:', this);
        items.classList.toggle('filter__items--closed');
    });
});
