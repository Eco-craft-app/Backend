using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events;
public record UserInfoProvided(Guid ProjectId, string UserName, string AvatarUrl) : INotification;