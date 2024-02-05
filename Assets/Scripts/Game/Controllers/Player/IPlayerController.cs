using System.Collections.Generic;
using Runner.Game.Models.Player;
using Runner.Game.Models.Token;
using Runner.Shared.Shared;
using UnityEngine;

namespace Runner.Game.Controllers.Player
{
    /// <summary>
    /// Интерфейс доступа в контроллеру игрока
    /// </summary>
    public interface IPlayerController
    {
        IPlayerModel PlayerModel { get; }
        Rigidbody2D Rigidbody { get; }
        Animator Animator { get; }
        
        /// <summary>
        /// Отдает список эффектов для параметра 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IReadOnlyList<ITokenEffectModel> GetParameterEffects(PlayerParameter parameter);
    }
}