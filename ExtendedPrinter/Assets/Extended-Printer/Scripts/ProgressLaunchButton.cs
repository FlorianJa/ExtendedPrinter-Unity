

using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Progress;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Examples.UX
{
    public class ProgressLaunchButton : MonoBehaviour
    {
        

        [Header("Which Indicator style is desired?")]
        [SerializeField]
        private IndicatorStyleEnum indicatorStyle = IndicatorStyleEnum.AnimatedOrbs;

        [Header("Which Progress style is desired?")]
        [SerializeField]
        private ProgressStyleEnum progressStyle = ProgressStyleEnum.None;

        /// <summary>
        /// Property that determines whether the indicator of the progress indicator
        /// is  None,  StaticIcon, AnimatedOrbs, or an instantiated Prefab.
        /// </summary>
        public IndicatorStyleEnum IndicatorStyle
        {
            get
            {
                return indicatorStyle;
            }

            set
            {
                indicatorStyle = value;
            }
        }

        /// <summary>
        /// Property indicating the Progress style:  None, Percentage, ProgressBar
        /// </summary>
        public ProgressStyleEnum ProgressStyle
        {
            get
            {
                return progressStyle;
            }

            set
            {
                progressStyle = value;
            }
        }

        public string UrlToFile;
        public MeshCreator MeshCreator;
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.OnButtonClicked += OnButtonClicked;
        }

        private void OnButtonClicked(GameObject obj)
        {
            if (ProgressIndicator.Instance.IsLoading)
            {
                return;
            }

            ProgressIndicator.Instance.Open(
                                         IndicatorStyleEnum.AnimatedOrbs,
                                         ProgressStyleEnum.None,
                                         ProgressMessageStyleEnum.Visible,
                                         "Loading");
            

            //Task.Run(() => LoadObject());
            StartCoroutine(LoadObject());
            


        }

        protected IEnumerator LoadObject()
        {
            yield return MeshCreator.LoadObject(UrlToFile);
        }
    }
}
