export function createResizable(ref, instance) {

    if (!ref) {
        return;
    }

    ref.resizeHandler = function () {
        let rect = ref.getBoundingClientRect();
        instance.invokeMethodAsync('Resize', rect.width, rect.height);
    };

    if (window.ResizeObserver) {
        ref.resizeObserver = new ResizeObserver(ref.resizeHandler);
        ref.resizeObserver.observe(ref);
    } else {
        window.addEventListener('resize', ref.resizeHandler);
    }

    let rect = ref.getBoundingClientRect();
    return {width: rect.width, height: rect.height};
}


export function destroyResizable(ref) {

    if (ref.resizeObserver) {
        ref.resizeObserver.disconnect();
        delete ref.resizeObserver;
    }

    if (ref.resizeHandler) {
        window.removeEventListener('resize', ref.resizeHandler);
        delete ref.resizeHandler;
    }
}