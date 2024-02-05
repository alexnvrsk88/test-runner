using Runner.Core.Services.Resource;
using Runner.Game.Models.Token;
using UnityEngine;

namespace Runner.Game.Controllers
{
    /// <summary>
    /// Контроллер монетки, содержит ссылку на на модель монетки для получения эффектов
    /// </summary>
    public sealed class TokenController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public ITokenModel TokenModel { get; private set; }
        
        private IResourcesService _resourcesService;

        public void Setup(ITokenModel tokenModel)
        {
            TokenModel = tokenModel;
        }
    }
}