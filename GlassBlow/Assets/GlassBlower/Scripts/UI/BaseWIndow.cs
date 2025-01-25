using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GlassBlower.Scripts.UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        protected UIController Control { get; private set; }

        public void Initialize(UIController game)
        {
            Control = game;
        }

        public virtual async UniTask Show(UniTask oldWindow)
        {
            await oldWindow;
            gameObject.SetActive(true);
            await UniTask.CompletedTask;
        }

        public virtual async UniTask Hide()
        {
            gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }
    }
}