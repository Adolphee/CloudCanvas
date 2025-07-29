
var gallery = document.querySelector('#GalleryInventory');
var carousel = document.querySelector('.carousel-inner');

(async function main() {
    try {
        const response = await fetch("http://localhost:7071/api/photos");
        if (!response.ok) throw new Error(`Error: ${response.status}`);
        const metadata = await response.json();
        updateGalleryItems(metadata);
    } catch (error) {
        // TODO: better error handling; alert, prompt for page reload or JS retry
        console.error("Failed to fetch metadata:", error.message);
    }
})();

function updateGalleryItems(metadata) {
    metadata.forEach(metaCard => {
        var oldCard = gallery.querySelector(`div:has(img[src="${metaCard.url}"])`);
        var carouselItem = carousel.querySelector(`div:has(img[src="${metaCard.url}"])`);
        if (oldCard != undefined) {
            updateCardMetadata(oldCard, metaCard)
        }
        if (carouselItem != undefined) {
            updateCarouselItemMetadata(carouselItem, metaCard);
        }
    });
}

function updateGalleryItemByIdentifier(metadata) {
    metadata.forEach(metaCard => {
        var oldCard = gallery.querySelector(`#${metaCard.identifier}`);
        var carouselItem = carousel.querySelector(`div:has(img[src="${metaCard.url}"])`);
        if (oldCard != undefined) {
            updateCardMetadata(oldCard, metaCard)
        }
        if (carouselItem != undefined) {
            updateCarouselItemMetadata(carouselItem, metaCard);
        }
    });
}

function updateCardMetadata(viewCard, cardMeta, withFooter = true) {
    var title = viewCard.querySelector('.card-title');
    title.textContent = cardMeta.originalFilename || 'No Title';

    var description = viewCard.querySelector('.card-text');
    description.textContent = cardMeta.description || 'No Description';

    if (withFooter) {
        var footer = viewCard.querySelector('.card-footer');
        footer.textContent = cardMeta.userId || 'Anonymous User';
    }
}

function updateCarouselItemMetadata(viewCard, cardMeta) {
    var title = viewCard.querySelector('.carousel-title');
    title.textContent = cardMeta.originalFilename || 'No Title';

    var description = viewCard.querySelector('.carousel-description');
    description.textContent = cardMeta.description || 'No Description';
}