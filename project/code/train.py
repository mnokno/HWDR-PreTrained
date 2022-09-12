import torch
import torch.nn as nn
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


# Calculates the accuracy on the test set
def test_accuracy(model: nn.Module) -> float:
    # Ensures the model is in eval mode
    model.eval()
    # We use a list to store accuracies as they are computed for each batch in the test_loader
    accuracies = []

    for data in test_loader:
        # Every data instance is an input + label pair
        X_batch, y_batch = data
        output = model(X_batch)
        accuracy_batch = (torch.max(output, dim=1)[1] == y_batch).type(torch.float32).mean().item()
        accuracies.append(accuracy_batch)

    # Calculates the average accuracy from all batches and returns it as a float
    return torch.Tensor(accuracies).mean().item()


# Defines what model to train or test
model_dir = "../saved_models/newModel.pt"
train = True
model_ = LeNet5Variant()

if train:  # Trains the model
    print("Initial accuracy:" + str(test_accuracy(model_)))
    loss_fn = torch.nn.CrossEntropyLoss()
    optimizer = torch.optim.SGD(model_.parameters(), lr=0.001, momentum=0.9)
    trn = MyTrainer(model_, optimizer, loss_fn)
    trn.fit(train_loader, test_loader, epochs=100, eval_every=1)
    torch.save(model_.state_dict(), model_dir)
    print("Finale accuracy: " + str(test_accuracy(model_)))

else:  # Tests the model
    model_.load_state_dict(torch.load(model_dir))
    print(test_accuracy(model_))
