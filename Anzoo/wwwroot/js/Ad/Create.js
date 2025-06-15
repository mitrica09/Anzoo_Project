// --------------------------------------------------------
//  Selectoare
// --------------------------------------------------------
const inputFile = document.getElementById('imageInput');
const imageContainer = document.getElementById('image-container');
const uploadLabel = document.getElementById('uploadLabel');

// --------------------------------------------------------
//  DataTransfer global – păstrează toate fișierele selectate
// --------------------------------------------------------
let dataTransfer = new DataTransfer();

// --------------------------------------------------------
//  Helpers UI
// --------------------------------------------------------
function updateMainBadge() {
    imageContainer.querySelectorAll('.image-item')
        .forEach(el => el.classList.remove('main'));

    const first = imageContainer.querySelector('.image-item');
    if (first) first.classList.add('main');
}

function updateAddBtn() {
    uploadLabel.style.display =
        (dataTransfer.files.length >= 8) ? 'none' : 'flex';
}

// --------------------------------------------------------
//  Adăugare imagini din <input type=file>
// --------------------------------------------------------
inputFile.addEventListener('change', () => {

    const selected = Array.from(inputFile.files);
    if (selected.length === 0) return;

    // Număr maxim de fișiere admise
    const freeSlots = 8 - dataTransfer.files.length;
    const toBeAdded = selected.slice(0, freeSlots);

    if (selected.length > freeSlots) {
        alert(`Puteți încărca maxim 8 imagini.\n
               Aveți deja ${dataTransfer.files.length}, 
               ați selectat ${selected.length}.`);
    }

    toBeAdded.forEach(file => {

        // 1) memorăm fișierul în DataTransfer
        dataTransfer.items.add(file);

        // 2) construim thumbnail-ul
        const thumb = document.createElement('div');
        thumb.className = 'image-item';
        thumb.file = file;                // păstrăm referința pt. Sortable

        const img = document.createElement('img');
        thumb.appendChild(img);

        const reader = new FileReader();
        reader.onload = e => (img.src = e.target.result);
        reader.readAsDataURL(file);

        // badge „principală”
        const badge = document.createElement('span');
        badge.className = 'main-badge';
        badge.textContent = 'PRINCIPALĂ';
        thumb.appendChild(badge);

        // buton X
        const remove = document.createElement('button');
        remove.type = 'button';
        remove.className = 'remove-btn';
        remove.innerHTML = '&times;';
        remove.addEventListener('click', () => {
            const index = Array.from(
                imageContainer.querySelectorAll('.image-item')
            ).indexOf(thumb);

            dataTransfer.items.remove(index);
            thumb.remove();

            inputFile.files = dataTransfer.files;
            updateMainBadge();
            updateAddBtn();
        });
        thumb.appendChild(remove);

        // inserăm înaintea label-ului „+”
        imageContainer.insertBefore(thumb, uploadLabel);
    });

    // 3) sincronizăm input-ul cu lista completă
    inputFile.files = dataTransfer.files;

    //  ⚠️ NU mai resetăm value – altfel pierdem fișierele!
    //      inputFile.value = '';

    updateMainBadge();
    updateAddBtn();
});

// --------------------------------------------------------
//  SortableJS – drag & drop
// --------------------------------------------------------
new Sortable(imageContainer, {
    animation: 150,
    draggable: '.image-item',
    filter: '.upload-label',
    onEnd: () => {
        const newDT = new DataTransfer();
        imageContainer.querySelectorAll('.image-item')
            .forEach(item => newDT.items.add(item.file));

        dataTransfer = newDT;
        inputFile.files = dataTransfer.files;

        // asigurăm că „+” rămâne ultimul
        imageContainer.appendChild(uploadLabel);

        updateMainBadge();
    }
});

// init
updateAddBtn();
