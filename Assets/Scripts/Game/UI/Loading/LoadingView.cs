using DG.Tweening;
using Plugins.AssetsReference;
using Runner.Game.Services.Preloading;
using Runner.Game.Services.UI;
using Runner.Game.Services.UI.Controllers;
using Runner.Game.Services.UI.Interfaces;
using TMPro;
using UnityEngine.UI;

namespace Runner.Game.UI.Loading
{
    [View("LoadingView", ViewLayer.Loader)]
    public sealed class LoadingView : ViewController
    {
        private const float ProgressAnimationDuration = .1f;
        private const float DotsAnimationDuration = .5f;

        private readonly IPreloadingService _preloadingService;
        
        private readonly string[] _dots = { "", ".", "..", "..." };

        private Slider _loadingProgressSlider;
        private TextMeshProUGUI _dotsText;
        private TextMeshProUGUI _versionText;

        private int _dotsIndex;
        private Sequence _dotsAnimationSequence;

        public LoadingView(IUnityComponent view,
            IUIService uiService,
            IPreloadingService preloadingService) : base(view, uiService)
        {
            _preloadingService = preloadingService;
        }

        protected override void OnInitialized()
        {
            _loadingProgressSlider = View.Get<Slider>("LoadingProgressSlider");
            _dotsText = View.Get<TextMeshProUGUI>("DotsText");
            
            _preloadingService.OnProgressChanged += HandleLoadingProgressChanged;
            _loadingProgressSlider.value = 0f;
        }

        protected override void OnShown()
        {
            _dotsIndex = 1;
            _dotsAnimationSequence = DOTween.Sequence()
                .AppendCallback(DotsAnimate)
                .AppendInterval(DotsAnimationDuration)
                .SetLoops(-1)
                .Play();
        }

        protected override void OnHide()
        {
            _dotsAnimationSequence.Kill();
            _dotsAnimationSequence = null;
        }

        private void HandleLoadingProgressChanged(float progress)
        {
            _loadingProgressSlider.DOValue(progress, ProgressAnimationDuration).SetEase(Ease.Linear);
        }

        private void DotsAnimate()
        {
            _dotsIndex++;
            if (_dotsIndex >= _dots.Length)
            {
                _dotsIndex = 0;
            }

            _dotsText.text = _dots[_dotsIndex];
        }
    }
}