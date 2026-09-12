// Prevent browser scroll restoration flicker BEFORE Angular boots
if ('scrollRestoration' in history) history.scrollRestoration = 'manual';
window.scrollTo(0, 0);
// Apply saved theme immediately to prevent flash of wrong theme
(function () {
  try {
    var t = localStorage.getItem('nn_theme');
    var dark = t ? t === 'dark' : true;
    document.documentElement.classList.add(dark ? 'theme-dark' : 'theme-light');
    if (!dark) document.documentElement.style.background = '#f5efe8';
  } catch (e) {}
})();
