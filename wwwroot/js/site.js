window.AppLoading = (function () {
    let activeRequests = 0;
    const defaultTitle = 'Loading...';
    const defaultText = 'Please wait a moment.';

    function getElements() {
        const overlay = document.getElementById('global-loading-overlay');
        if (!overlay) {
            return null;
        }

        return {
            overlay,
            title: overlay.querySelector('.global-loading-title'),
            text: overlay.querySelector('.global-loading-text')
        };
    }

    function renderMessage(options) {
        const elements = getElements();
        if (!elements) {
            return;
        }

        elements.title.textContent = options?.title || defaultTitle;
        elements.text.textContent = options?.text || defaultText;
    }

    function syncVisibility() {
        const elements = getElements();
        if (!elements) {
            return;
        }

        const isVisible = activeRequests > 0;
        elements.overlay.classList.toggle('is-visible', isVisible);
        elements.overlay.setAttribute('aria-hidden', isVisible ? 'false' : 'true');
        document.body.classList.toggle('app-loading', isVisible);

        if (!isVisible) {
            renderMessage();
        }
    }

    function show(options) {
        activeRequests++;
        renderMessage(options);
        syncVisibility();
    }

    function hide(force) {
        activeRequests = force ? 0 : Math.max(0, activeRequests - 1);
        syncVisibility();
    }

    function shouldHandleRequest(settings) {
        return settings?.showGlobalLoader !== false;
    }

    $(document).ajaxSend(function (_event, _jqXHR, settings) {
        if (!shouldHandleRequest(settings)) {
            return;
        }

        show({
            title: settings.loaderTitle,
            text: settings.loaderText
        });
    });

    $(document).ajaxComplete(function (_event, _jqXHR, settings) {
        if (!shouldHandleRequest(settings)) {
            return;
        }

        hide();
    });

    return {
        show,
        hide
    };
})();

window.showGlobalLoader = function (options) {
    window.AppLoading.show(options);
};

window.hideGlobalLoader = function (force) {
    window.AppLoading.hide(force);
};

$(document).ready(function () {
    $('#menuModules .nav-link').on('click', function (e) {
        e.preventDefault();

        $('#menuModules .nav-link').removeClass('active');
        $(this).addClass('active');

        const modulo = $(this).data('module');
        $('.page-title').text('Módulo: ' + capitalizeText(modulo));
        $('.page-text').text('Has seleccionado el módulo "' + capitalizeText(modulo) + '".');
    });

    function capitalizeText(text) {
        return text.charAt(0).toUpperCase() + text.slice(1);
    }
});
