document.querySelectorAll('.promotion-card').forEach(card => {
    const filledBar = card.querySelector('.promotion-filledbar');
    const targetWidth = card.getAttribute('data-fill') + '%';

    card.addEventListener('mouseenter', () => {
        filledBar.style.width = targetWidth;
    });

    card.addEventListener('mouseleave', () => {
        filledBar.style.width = '0%';
    });
});