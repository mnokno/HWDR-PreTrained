from typing import Tuple, List
import torch
import torch.optim as optim
from torch.optim import lr_scheduler
import numpy as np
from torch import Tensor
import torch.nn as nn
import torch.nn.functional as F
from torch.nn.modules.loss import _Loss
import torchvision
from torchvision.datasets import MNIST
import torchvision.transforms as transforms
from torch.utils.data import DataLoader
from HWDR_model import MyConNet1, MyConNet2, MyConNet3, LeNet5Variant
from trainer import MyTrainer

# Defines transformations for dataset normalization
img_transforms = transforms.Compose([
    transforms.ToTensor(),
    transforms.Normalize((0.1305,), (0.3081,))
])

# Downloads training data set if not already downloaded (MINTs) and assigns transformation
# https://pytorch.org/docs/stable/data.html
train_dataset = MNIST(root='../mnist_data/',
                      train=True,
                      download=True,
                      transform=img_transforms)

test_dataset = MNIST(root='../mnist_data/',
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

def test_accuracy(model):
    model.eval()
    accuracies = []
    for data in test_loader:
        # Every data instance is an input + label pair
        X_batch, y_batch = data
        output = model(X_batch)
        accuracy_batch = (torch.max(output, dim=1)[1] == y_batch).type(torch.float32).mean().item()
        accuracies.append(accuracy_batch)
    return torch.Tensor(accuracies).mean().item()

model = MyConNet3()
model.load_state_dict(torch.load("../saved_models/newModel.pt"))
#loss_fn = torch.nn.CrossEntropyLoss()
#optimizer = torch.optim.SGD(model.parameters(), lr=0.001, momentum=0.9)

print(test_accuracy(model))

#trn = MyTrainer(model, optimizer, loss_fn)
#trn.fit(train_loader, test_loader, epochs=100, eval_every=5)
#torch.save(model.state_dict(), "../saved_models/newModel.pt")

#print(test_accuracy(model))
