document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".favorite-form").forEach(form => {
        form.addEventListener("submit", async e => {
            e.preventDefault();

            // ⛔ Dacă utilizatorul nu e logat, redirecționează către Login
            if (typeof window.isAuthenticated !== "undefined" && !window.isAuthenticated) {
                window.location.href = window.loginUrl || "/Account/Login";
                return;
            }


            const button = form.querySelector(".favorite-btn");
            const heartIcon = button?.querySelector("i");
            const navbarHeart = document.querySelector(".navbar-heart-icon");
            const adId = form.querySelector("input[name='adId']")?.value;

            if (!adId || !heartIcon || !navbarHeart) return;

            // ✅ Setăm icon plin
            heartIcon.classList.remove("bi-heart");
            heartIcon.classList.add("bi-heart-fill");

            // 💫 Animăm inima
            const start = heartIcon.getBoundingClientRect();
            const end = navbarHeart.getBoundingClientRect();

            const flyingHeart = heartIcon.cloneNode(true);
            flyingHeart.classList.add("flying-heart");
            flyingHeart.style.left = `${start.left}px`;
            flyingHeart.style.top = `${start.top}px`;

            document.body.appendChild(flyingHeart);

            requestAnimationFrame(() => {
                flyingHeart.style.transform = `translate(${end.left - start.left}px, ${end.top - start.top}px) scale(0.4)`;
                flyingHeart.style.opacity = "0";
            });

            setTimeout(() => flyingHeart.remove(), 700);

            // 📡 AJAX POST
            const response = await fetch(form.action, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "RequestVerificationToken": document.querySelector("input[name='__RequestVerificationToken']")?.value ?? ""
                },
                body: `adId=${encodeURIComponent(adId)}`
            });

            // ✅ Verificare răspuns
            if (response.ok) {
                button.disabled = true; // prevenim dublu click
            }
        });
    });
});
