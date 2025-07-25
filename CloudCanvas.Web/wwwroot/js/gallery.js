
var gallery = document.querySelector('#GalleryInventory');
gallery.textContent = '';

(async function main() {
    try {
        const response = await fetch("http://localhost:7071/api/GetBlobs");
        if (!response.ok) throw new Error(`Error: ${response.status}`);
        const metadata = await response.json();
        metadata.forEach(card => addCard(card));
    } catch (error) {
        // TODO: better error handling; alert, prompt for page reload or JS retry
        console.error("Failed to fetch metadata:", error.message);
    }
})();

function addCard(item) {
    // I could have pasted HTML here but this slower, 
    // more boilerplate method is the safest(think of XSS attack)
    var newCard = document.createElement('div');
    newCard.classList.add('card');
    newCard.style = 'width: 18rem;';

    // Image
    var img = document.createElement('img');
    img.src = item.url;
    img.className = 'd-block w-100';
    img.style.height = '100px';

    // Body
    var body = document.createElement('div');
    body.className = 'card-body';

    var title = document.createElement('h6');
    title.className = 'card-title';
    title.textContent = item.name ?? 'No Title';

    var description = document.createElement('p');
    description.className = 'card-text';
    description.textContent = item.description ?? 'No Description';

    body.appendChild(title);
    body.appendChild(description);

    // Footer
    var footer = document.createElement('div');
    footer.className = 'card-footer';
    footer.textContent = item.userId ?? 'Anonymous User';

    // Append everything
    newCard.appendChild(img);
    newCard.appendChild(body);
    newCard.appendChild(footer);
    gallery.appendChild(newCard);
}