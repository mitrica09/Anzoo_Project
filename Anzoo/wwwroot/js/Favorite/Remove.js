document.addEventListener("DOMContentLoaded", () => {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    document.querySelectorAll(".remove-favorite-btn").forEach(button => {
        button.addEventListener("click", async (e) => {
            e.preventDefault(); // ⛔️ împiedică form submit default

            const adId = button.dataset.adId;
            const url = button.dataset.url;
            const row = button.closest(".ad-row-wrapper");

            if (!adId || !url || !row) return;

            // 📨 Trimite request
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "RequestVerificationToken": token
                },
                body: `adId=${encodeURIComponent(adId)}`
            });

            if (response.ok) {
                row.style.transition = "opacity 0.4s ease, transform 0.4s ease";
                row.style.opacity = "0";
                row.style.transform = "translateX(20px)";
                setTimeout(() => row.remove(), 400);
            }
        });

    });
});