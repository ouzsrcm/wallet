using walletv2.Data.Entities.DataTransferObjects;

namespace walletv2.Dtos;


public class UserDetailResponseDto : BaseResponseDto
{
    public UserDetailedInfoDto? User { get; set; }
}
