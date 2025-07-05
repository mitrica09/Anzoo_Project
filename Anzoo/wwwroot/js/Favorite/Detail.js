document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector(".favorite-form");

    if (!form) return;

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const button = form.querySelector(".favorite-btn");
        const icon = button.querySelector("i");
        const adId = form.querySelector("input[name='adId']")?.value;
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        if (!adId || !icon) return;

        const isFavorite = icon.classList.contains("bi-heart-fill");
        const action = isFavorite ? "RemoveFromFavorites" : "AddToFavorites";

        const response = await fetch(`/Favorite/${action}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
                "RequestVerificationToken": token
            },
            body: `adId=${encodeURIComponent(adId)}`
        });

        if (response.ok) {
            icon.classList.toggle("bi-heart");
            icon.classList.toggle("bi-heart-fill");

            button.innerHTML = icon.outerHTML + (isFavorite ? " Salvează în favorite" : " Șterge din favorite");
        }
    });
});
