// --------------  Selectoare & variabile  ----------------
const imageContainer = document.getElementById('image-container');
const uploadLabel = document.getElementById('uploadLabel');
const inputFile = document.getElementById('imageInput');
const mainImageInput = document.getElementById('mainImageInput');

let dataTransfer = new DataTransfer();           // fișiere NOI

// --------------  Helpers UI  ----------------
function refreshMainBadge() {
    imageContainer.querySelectorAll('.image-item')
        .forEach(el => el.classList.remove('main'));

    const first = imageContainer.querySelector('.image-item');
    if (first) {
        first.classList.add('main');
        const mainFile = first.dataset.filename;
        if (mainImageInput) {
            mainImageInput.value = mainFile;
        }
    }
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
        const inp = document.createElement('input');
        inp.type = 'hidden';
        inp.name = 'ImagesToDelete';
        inp.value = fileName;
        imageContainer.appendChild(inp);
    } else {
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
        div.className = 'image-item';
        div.file = file;

        const img = document.createElement('img');
        div.appendChild(img);
        const reader = new FileReader();
        reader.onload = e => img.src = e.target.result;
        reader.readAsDataURL(file);

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
        // 1. Elimină inputurile vechi
        document.querySelectorAll('input[name="ExistingImages"]').forEach(i => i.remove());

        // 2. Creează inputuri noi în ORDINEA ACTUALĂ
        const allImageItems = imageContainer.querySelectorAll('.image-item');

        allImageItems.forEach(div => {
            const filename = div.dataset.filename;
            if (!filename) return;

            const hidden = document.createElement('input');
            hidden.type = 'hidden';
            hidden.name = 'ExistingImages';
            hidden.value = filename;
            imageContainer.appendChild(hidden);
        });

        refreshMainBadge();
    }
});

// --------------  Init --------------
refreshMainBadge();
updateAddBtn();
