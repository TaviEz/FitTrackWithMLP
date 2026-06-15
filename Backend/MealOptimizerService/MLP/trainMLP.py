import torch
import pandas as pd
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, TensorDataset
from mealSelectorMLP import MealSelectorMLP
from sklearn.preprocessing import StandardScaler
import joblib

def prepare_data():
    df = pd.read_csv("./data/meal_mlp_dataset.csv")
    X_raw = df[['calories', 'protein', 'meal_type_id']].values
    y_raw = df['meal_id_suggestion'].values

    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X_raw)
    joblib.dump(scaler, './data/scaler.joblib')

    # Create mapping: Model Index (0-N) -> Real Meal ID
    unique_ids = sorted(df['meal_id_suggestion'].unique())
    id_map = {original_id: i for i, original_id in enumerate(unique_ids)}
    reverse_map = {i: original_id for i, original_id in enumerate(unique_ids)}
    
    y_mapped = [id_map[val] for val in y_raw]

    return torch.FloatTensor(X_scaled), torch.LongTensor(y_mapped), reverse_map

# Prepare Data and Mapping
X_train, y_train, reverse_map = prepare_data()
num_unique_meals = len(reverse_map)

# Init Model
model = MealSelectorMLP(num_unique_meals)
criterion = nn.CrossEntropyLoss()
optimizer = optim.Adam(model.parameters(), lr=0.001)

dataset = TensorDataset(X_train, y_train)
loader = DataLoader(dataset, batch_size=32, shuffle=True)

# Training Loop
print("Training started...")
for epoch in range(2001):
    total_loss = 0.0
    for batch_X, batch_y in loader:
        optimizer.zero_grad()
        outputs = model(batch_X)
        loss = criterion(outputs, batch_y)
        loss.backward()
        optimizer.step()
        total_loss += loss.item()
    
    if epoch % 10 == 0:
        avg_loss = total_loss / len(loader)
        print(f"Epoch {epoch}, Loss: {avg_loss:.4f}")

# Evaluation
model.eval() 
with torch.no_grad():
    all_outputs = model(X_train)

    # Top 1
    _, predicted = torch.max(all_outputs, 1)
    accuracy = (predicted == y_train).sum().item() / y_train.size(0)
    print(f"Top-1 Training Accuracy: {accuracy * 100:.2f}%")

    # Top k
    def topk_acc(outputs, targets, k):
        _, topk_preds = torch.topk(outputs, k, dim=1)
        correct = topk_preds.eq(targets.view(-1, 1).expand_as(topk_preds))
        return correct.any(dim=1).float().mean().item()
    
    print(f"Top-5 Training Accuracy:  {topk_acc(all_outputs, y_train, 5)  * 100:.2f}%")
    print(f"Top-15 Training Accuracy: {topk_acc(all_outputs, y_train, 15) * 100:.2f}%")

# Save Model and Metadata (Mapping + Class Count)
torch.save(model.state_dict(), "./data/meal_selector_model.pth")

joblib.dump(reverse_map, './data/id_mapping.joblib')

print("All files saved (model, scaler, metadata).")