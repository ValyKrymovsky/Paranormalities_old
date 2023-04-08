using UnityEngine;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;

public class P_PlayerModelLoader : MonoBehaviour
{
    [SerializeField] private string avatarUrl = "https://models.readyplayer.me/642f13ce204b1d02c9d7bc80.glb";
    private GameObject avatar;

        private void Start()
        {
            ApplicationData.Log();
            var avatarLoader = new AvatarObjectLoader();
            avatarLoader.OnCompleted += (_, args) =>
            {
                avatar = args.Avatar;
                AvatarAnimatorHelper.SetupAnimator(args.Metadata.BodyType, avatar);
            };
            avatarLoader.LoadAvatar(avatarUrl);
        }

        private void OnDestroy()
        {
            if (avatar != null) Destroy(avatar);
        }
}
