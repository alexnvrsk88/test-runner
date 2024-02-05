using System.Collections.Generic;

namespace Runner.Game.Models.Token
{
    /// <summary>
    /// Модель монетки, содержит список эффектов и визуал (префаб)
    /// </summary>
    public interface ITokenModel
    {
        string PrefabName { get; }
        IReadOnlyList<ITokenEffectModel> Effects { get; }
    }
}