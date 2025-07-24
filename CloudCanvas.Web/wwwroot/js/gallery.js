import { forEach } from "core-js/library/core/array";

console.log("START Gallery");

try {
    const response = await fetch("http://localhost:7071/api/GetBlobs");
    if (!response.ok) throw new Error(`Error: ${response.status}`);
    const metadata = await response.json();

    
    console.log(metadata);
} catch (error) {
    console.error("Failed to fetch metadata:", error);
}


function populateMetadataFields(metadata) {
    var cards = document.querySelectorAll('#GalleryInventory > .card');
    for (var i = 0; i < metadata.length; i++) {
        var title = cards[i].querySelector(`#card-${i}-title`);
        var description = cards[i].querySelector(`#card-${i}-description`);
        var footer = cards[i].querySelector(`#card-${i}-footer`);
        title.textContent = metadata.name;
        description.textContent = metadata.description;
        footer.textContent = metadata.
        console.log(`Altered card ${i}`)
    }
}
console.log("DONE Gallery")