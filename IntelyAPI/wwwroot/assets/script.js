(function () {
    var link = document.querySelector("link[rel*='icon']") || document.createElement('link');;
    document.head.removeChild(link);
    link = document.querySelector("link[rel*='icon']") || document.createElement('link');
    document.head.removeChild(link);
    link = document.createElement('link');
    link.type = 'image/png';
    link.rel = 'shortcut icon';
    link.href = '../image/DSGIcon.png';
    document.getElementsByTagName('head')[0].appendChild(link);
})();
