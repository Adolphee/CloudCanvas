console.log("START Gallery");

(async function main() {
    try {
        const response = await fetch("http://localhost:7071/api/GetBlobs");
        if (!response.ok) throw new Error(`Error: ${response.status}`);
        const metadata = await response.json();
        console.log(metadata);
        populateMetadataFields(metadata);

    } catch (error) {
        console.error("Failed to fetch metadata:", error.message);
    }
})();

function populateMetadataFields(metadata) {
    var cards = document.querySelectorAll('#GalleryInventory > .card');
    console.log(metadata);
    for (var j = 0; j < metadata.length; j++) {
        var card = document.querySelector(`.card img[src="${metadata[j].url}"]`).parentElement;
        if (card) {
            var title = card.querySelector('.card-title');
            var description = card.querySelector('.card-text');
            var footer = card.querySelector('.card-footer');
            title.textContent = metadata[j].name;
            description.textContent = metadata[j].description;
            footer.textContent = metadata[j].userId ?? "Anonymous User";
        }
        console.log(`Altered card ${j}`)
    }
}
console.log("DONE Gallery");