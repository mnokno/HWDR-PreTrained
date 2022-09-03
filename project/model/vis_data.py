import torch
import torchvision
import torchvision.transforms as transforms
import matplotlib.pyplot as plt
import numpy as np

# Defines transformations for dataset normalization
img_transforms = transforms.Compose([
    transforms.ToTensor(),
    transforms.Normalize((0.1305,), (0.3081,))
])

# Downloads training data set if not already downloaded (MINTs) and assigns transformation
# https://pytorch.org/docs/stable/data.html
train_dataset = torchvision.datasets.MNIST(root='../mnist_data/',
                                           train=True,
                                           download=True,
                                           transform=img_transforms)

test_dataset = torchvision.datasets.MNIST(root='../mnist_data/',
                                          train=False,
                                          download=True,
                                          transform=img_transforms)

# Creates data loaders
train_loader = torch.utils.data.DataLoader(dataset=train_dataset,
                                           batch_size=60,
                                           shuffle=True)

test_loader = torch.utils.data.DataLoader(dataset=test_dataset,
                                          batch_size=60,
                                          shuffle=False)

# Class labels
classes = ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9')


# Helper function for inline image display
def matplotlib_imshow(img, one_channel=False):
    if one_channel:
        img = img.mean(dim=0)
    img = img / 2 + 0.5  # unnormalize
    npimg = img.numpy()
    if one_channel:
        plt.imshow(npimg, cmap="Greys")
    else:
        plt.imshow(np.transpose(npimg, (1, 2, 0)))


dataiter = iter(train_loader)
images, labels = dataiter.next()

# Create a grid from the images and show them
img_grid = torchvision.utils.make_grid(images)
matplotlib_imshow(img_grid, one_channel=True)
print('  '.join(classes[labels[j]] for j in range(4)))
