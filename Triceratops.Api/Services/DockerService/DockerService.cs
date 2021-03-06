﻿using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DockerService
{
    public class DockerService : IDockerService
    {
        private readonly DockerClient _dockerClient;

        public DockerService() : this(CreateDockerClient())
        {
        }

        public DockerService(DockerClient client)
        {
            _dockerClient = client;
        }


        public async Task<string> CreateContainerAsync(string imageName, string containerName, IEnumerable<string> env = default)
        {
            try
            {
                var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = imageName,
                    Name = containerName,
                    Env = new List<string>(env),
                });

                if (response.Warnings.Any())
                {
                    // Do some stuff with the warnings.

                    Debug.WriteLine($"{response.Warnings.Count} warnings were generated");                
                }

                return response.ID;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to create container: {exception.Message}");

                return null;
            }
        }

        public async Task StopContainerAsync(string containerId)
        {
            try
            {
                await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to stop container: {exception.Message}");
            }
        }

        public async Task DeleteContainerAsync(string containerId, bool force = false)
        {
            try
            {
                await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
                {
                    Force = force
                });
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to delete container: {exception.Message}");
            }
        }

        public async Task DownloadImageAsync(string imageName, string version = "latest")
        {
            try
            {
                await _dockerClient.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = imageName,
                        Tag = version,
                    },
                    null,
                    new Progress<JSONMessage>(m => Debug.WriteLine(m.ProgressMessage))
                );
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to download image: {exception.Message}");
            }
        }

        public async Task RunContainerAsync(string containerId, params string[] parameters)
        {
            try
            {
                await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to run container: {exception.Message}");
            }
        }

        private static DockerClient CreateDockerClient()
        {
            return new DockerClientConfiguration(new Uri("tcp://192.168.99.100:2375"))
                .CreateClient();    
        }
    }
}
