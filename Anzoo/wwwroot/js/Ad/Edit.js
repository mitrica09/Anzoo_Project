// --------------  Selectoare & variabile  ----------------
const imageContainer = document.getElementById('image-container');
const uploadLabel = document.getElementById('uploadLabel');
const inputFile = document.getElementById('imageInput');

let dataTransfer = new DataTransfer();           // fișiere NOI

// --------------  Helpers UI  ----------------
function refreshMainBadge() {
    imageContainer.querySelectorAll('.image-item')
        .forEach(el => el.classList.remove('main'));
    const first = imageContainer.querySelector('.image-item');
    if (first) first.classList.add('main');
}
function updateAddBtn() {
    const count = imageContainer.querySelectorAll('.image-item').length;
    uploadLabel.style.display = (count >= 8 ? 'none' : 'flex');
}

// --------------  ȘTERGERE imagine  ----------------
imageContainer.addEventListener('click', e => {
    if (!e.target.classList.contains('remove-btn')) return;

    const item = e.target.closest('.image-item');
    const fileName = item.dataset.filename;

    if (item.classList.contains('existing')) {
        // ► notăm pt. ștergere
        const inp = document.createElement('input');
        inp.type = 'hidden';
        inp.name = 'ImagesToDelete';
        inp.value = fileName;
        imageContainer.appendChild(inp);
    } else {
        // ► era o imagine nouă → o scoatem din DataTransfer
        const idx = Array.from(
            imageContainer.querySelectorAll('.image-item')
        ).filter(el => !el.classList.contains('existing'))
            .indexOf(item);
        dataTransfer.items.remove(idx);
        inputFile.files = dataTransfer.files;
    }

    item.remove();
    refreshMainBadge();
    updateAddBtn();
});

// --------------  ADAUGĂ imagini noi  ----------------
inputFile.addEventListener('change', () => {
    const sel = Array.from(inputFile.files);
    if (!sel.length) return;

    const free = 8 - imageContainer.querySelectorAll('.image-item').length;
    const toAdd = sel.slice(0, free);

    toAdd.forEach(file => {
        dataTransfer.items.add(file);

        const div = document.createElement('div');
        div.className = 'image-item';   // nu are .existing
        div.file = file;

        const img = document.createElement('img');
        div.appendChild(img);
        new FileReader().onload = e => img.src = e.target.result;
        new FileReader().readAsDataURL?.(file);

        const badge = document.createElement('span');
        badge.className = 'main-badge';
        badge.textContent = 'PRINCIPALĂ';
        div.appendChild(badge);

        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'remove-btn';
        btn.innerHTML = '&times;';
        div.appendChild(btn);

        imageContainer.insertBefore(div, uploadLabel);
    });

    inputFile.files = dataTransfer.files;
    refreshMainBadge();
    updateAddBtn();
});

// --------------  Drag & drop (SortableJS)  --------------
new Sortable(imageContainer, {
    animation: 150,
    draggable: '.image-item',
    filter: '.upload-label',
    onEnd: () => {
        // ► reconstruim input-urile ExistingImages în noua ordine
        imageContainer.querySelectorAll('input[name="ExistingImages"]').forEach(i => i.remove());

        imageContainer.querySelectorAll('.image-item.existing')
            .forEach(div => {
                const inp = document.createElement('input');
                inp.type = 'hidden';
                inp.name = 'ExistingImages';
                inp.value = div.dataset.filename;
                imageContainer.appendChild(inp);
            });

        refreshMainBadge();
    }
});

// --------------  Init --------------
refreshMainBadge();
updateAddBtn();
