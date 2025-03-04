document.addEventListener("DOMContentLoaded", async () => {
    var user = await getUser();
    var div = document.querySelectorAll('.tyn-media.tyn-size-lg.name');
    div.forEach(d => {
        var img = d.querySelector('img');
        if (img) {
            img.src = user.avatar_url;
        }
    })
    var nameUserH6 = document.querySelector('#name-user');
    nameUserH6.textContent = user.nick_name;
    var logout = document.querySelector('#log-out');
    logout.addEventListener('click', function (event) {
        event.preventDefault();
        document.cookie = 'bearer_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
        setTimeout(function () {
            window.location.href = '/';
        }, 300);
    });
})