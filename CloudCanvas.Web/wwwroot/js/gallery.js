var gallery = $('#GalleryInventory');
var carousel = $('.carousel-inner'); 
const api = "http://localhost:7071/api/photos/"; 
var metadata;

(async function main() {
    await axios(api)
    .then(response => {
        metadata = response.data;
        metadata.forEach(async item => await configureModals(item));
    });
})();

async function fetchGalleryItems(url) {
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Error: ${response.status}`);
        const metadata = await response.json();
        return metadata;
    } catch (error) {
        // TODO: better error handling; alert, prompt for page reload or JS retry
        console.error("Failed to fetch metadata:", error.message);
    }
}

async function saveGalleryItem(item) {
    try {
        const response = await fetch(url + item.id, "patch", item);
        if (!response.ok) throw new Error(`Error: ${response.status}`);
        const metadata = await response.json();
        return metadata;
    } catch (error) {
        // TODO: better error handling; alert, prompt for page reload or JS retry
        console.error("Failed to fetch metadata:", error.message);
    }
}

async function deleteItemAsync(item) {
    var id = item.id;
    var userId = item.userId;
    var request = { method: "delete", body: JSON.stringify(buildPayload(item)) };
    const response = await fetch(api + id, request);
    if (!response.ok) throw new Error(`Error: ${response.status}`);
}

function updateTableRow(item) {
    var tr = $(`#tr_${item.id}`);
    var displayName = tr.find('.tr-displayName');
    displayName.text(item.displayName ?? displayName.text());

    var description = tr.find('.tr-description');
    description.text(item.description ?? description.text());
}

function configureModals(item) {
    var deleteBtn = $(`#Del-${item.id} .btn-delete-confirm`);
    deleteBtn.on('click', async (btn) => {
        var dataset = collectDataset(btn);
        await deleteItemAsync(dataset);
        deleteTableRow(`tr_${dataset.id}`);
        deleteCarouselItem(item);
    });

    var saveChangesBtn = $(`#Mod-${item.id} .btn-edit-item`);
    saveChangesBtn.on("click", async (btn) => {
        var set = collectDataset(btn);
        var payload = buildPayload(set);
        var res = await axios.patch(api + set.id, payload);
        if (res) {
            item.displayName = res.data.displayName;
            item.description = res.data.description;
            updateTableRow(item);
            updateCarouselItem(item);
        }
    });
}

function buildPayload(dataset) {
    return {
        "id": dataset.id,
        "userId": dataset.userId,
        "displayName": $(`#title-${dataset.id}`).val(),
        "description": $(`#caption-${dataset.id}`).val()
    }
}

function collectDataset(btn) {
    return {
        "id": btn.currentTarget.dataset.identifier,
        "userId": btn.currentTarget.dataset.userId,
        "container": btn.currentTarget.dataset.container
    }
}

function deleteTableRow(tr_id) {
    var tr = document.getElementById(tr_id);
    metadata = metadata.filter(item => "tr_" + item.id != tr_id);
    if (tr) tr.remove();
}

function deleteCarouselItem(item) {
    var carouselItem = carousel.find(`div:has(img[src="${item.url}"])`);
    if (carouselItem) carouselItem.remove();
}

function updateCarousel(metadata) {
    metadata.forEach(item => updateCarouselItem(item));
}

function updateCarouselItem(item) {
    var carouselItem = carousel.find(`div:has(img[src="${item.url}"])`);
    if (carouselItem != undefined) {
        var title = carouselItem.find('.carousel-title');
        title.textContent = item.originalFilename || 'No Title';

        var description = carouselItem.find('.carousel-description');
        description.textContent = item.description || 'No Description';
    }
}

