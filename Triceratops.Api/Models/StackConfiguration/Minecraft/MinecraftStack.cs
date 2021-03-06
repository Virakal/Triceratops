﻿using System.Threading.Tasks;
using Triceratops.Api.Models.Persistence.Stacks;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Models.StackConfiguration.Minecraft
{
    public class MinecraftStack : AbstractStack
    {
        private const string ContainerName = "minecraft";

        private const string ImageName = "itzg/minecraft-server";

        public MinecraftStack(IDockerService dockerService, ContainerStack stack)
            : base(dockerService, stack)
        {
            AddImage(ImageName);
        }

        public override async Task BuildAsync()
        {
            await DownloadImagesAsync();

            await CreateContainerBuilder(ImageName, ContainerName)
                .UsePrefix()
                .CreateContainerAsync();
        }
    }
}
