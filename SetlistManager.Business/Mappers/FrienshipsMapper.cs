using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Mappers;

public static class FriendshipsMapper
{
    public static FriendModel ToModel(this Friendship friendship, int currentUserId)
    {
        var friendUser = friendship.InitiatorId == currentUserId
            ? friendship.Reciever
            : friendship.Initiator;

        return new FriendModel
        {
            Id = friendUser.Id,
            FriendshipId = friendship.Id,
            Username = friendUser.UserName!,
            InitiatedById = friendship.InitiatorId,
            State = friendship.State
        };
    }

    public static Friendship ToEntity(this FriendshipRequestModel model, int currentUserId)
    {
        return new Friendship
        {
            InitiatorId = currentUserId,
            RecieverId = model.RecieverId!.Value,
            State = FriendshipState.Pending
        };
    }
}