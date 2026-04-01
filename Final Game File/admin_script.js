import { initializeApp } from 'https://www.gstatic.com/firebasejs/12.11.0/firebase-app.js';
import { getFirestore, collection, query, where, getDocs, doc, getDoc, setDoc, deleteDoc } from 'https://www.gstatic.com/firebasejs/12.11.0/firebase-firestore.js';
import { getAuth, createUserWithEmailAndPassword } from 'https://www.gstatic.com/firebasejs/12.11.0/firebase-auth.js';

// Firebase configuration 
const firebaseConfig = {
    apiKey: "REPLACE_WITH_FIREBASE_API_KEY",
    authDomain: "unity-cyber-game.firebaseapp.com",
    databaseURL: "https://unity-cyber-game-default-rtdb.asia-southeast1.firebasedatabase.app",
    projectId: "unity-cyber-game",
    storageBucket: "unity-cyber-game.firebasestorage.app",
    messagingSenderId: "1068419443054",
    appId: "1:1068419443054:web:88c1edfadb4cfb7880b97f",
    measurementId: "G-F4VV3K45BR"
};

// Initialize Firebase 
const app = initializeApp(firebaseConfig);
const db = getFirestore(app);
const auth = getAuth(app);

console.log("Firestore and Auth initialized");

// Global variables
let usersList = [];
let currentSelectedUserId = null;
let adminsList = [];
let currentSelectedAdminId = null;

// Function to escape HTML to prevent XSS
function escapeHtml(str) {
    if (str === null || str === undefined) return '';
    const div = document.createElement('div');
    div.textContent = String(str);
    return div.innerHTML;
}

// Function to format time
function formatTime(timeValue) {
    if (timeValue === null || timeValue === undefined) {
        return 'Not available';
    }
    
    if (typeof timeValue === 'number') {
        if (timeValue < 60) {
            return `${timeValue} seconds`;
        } else {
            const minutes = Math.floor(timeValue / 60);
            const seconds = Math.round(timeValue % 60);
            return `${minutes} minute${minutes !== 1 ? 's' : ''} and ${seconds} second${seconds !== 1 ? 's' : ''}`;
        }
    }
    
    return String(timeValue);
}

// Data order
const fieldOrder = [
    'mapBoundary',
    'playerPosition',
    'questProgressData',
    'HotbarSaveData',
    'chestSaveData'
];

// Recursive function to display nested objects dynamically with ordering
function displayNestedObject(obj, level = 0) {
    if (obj === null || obj === undefined) {
        return '<span class="empty-state">null</span>';
    }
    
    if (typeof obj !== 'object') {
        return `<span class="nested-value">${escapeHtml(String(obj))}</span>`;
    }
    
    if (Array.isArray(obj)) {
        if (obj.length === 0) return '<span class="empty-state">Empty array</span>';
        let html = '<div class="nested-object">';
        obj.forEach((item, index) => {
            html += `<div class="nested-item">
                        <span class="nested-key">[${index}]:</span>
                        ${displayNestedObject(item, level + 1)}
                     </div>`;
        });
        html += '</div>';
        return html;
    }
    
    // Handle object - sort keys by custom order
    const keys = Object.keys(obj);
    if (keys.length === 0) return '<span class="empty-state">Empty object</span>';
    
    // Sort keys based on fieldOrder
    const sortedKeys = [...keys].sort((a, b) => {
        const indexA = fieldOrder.indexOf(a);
        const indexB = fieldOrder.indexOf(b);
        
        if (indexA !== -1 && indexB !== -1) {
            return indexA - indexB;
        }
        if (indexA !== -1) return -1;
        if (indexB !== -1) return 1;
        return a.localeCompare(b);
    });
    
    let html = '<div class="nested-object">';
    sortedKeys.forEach(key => {
        const value = obj[key];
        html += `<div class="nested-item">
                    <span class="nested-key">${escapeHtml(key)}:</span>`;
        
        if (value && typeof value === 'object') {
            html += displayNestedObject(value, level + 1);
        } else {
            html += `<span class="nested-value">${escapeHtml(String(value))}</span>`;
        }
        html += '</div>';
    });
    html += '</div>';
    return html;
}

// ========== USER SETTINGS FUNCTIONS ==========

// Function to fetch users with role "user"
async function fetchUsers() {
    const userSelect = document.getElementById('user-select');
    userSelect.innerHTML = '<option value="">Loading users...</option>';
    
    try {
        const usersRef = collection(db, 'users');
        const q = query(usersRef, where('role', '==', 'user'));
        const querySnapshot = await getDocs(q);
        
        if (querySnapshot.empty) {
            userSelect.innerHTML = '<option value="">No users found with role "user"</option>';
            console.log('No users found with role "user"');
            return;
        }
        
        userSelect.innerHTML = '<option value="">Select a user...</option>';
        
        usersList = [];
        querySnapshot.forEach((doc) => {
            const userData = doc.data();
            usersList.push({
                id: doc.id,
                ...userData
            });
            
            const option = document.createElement('option');
            option.value = doc.id;
            const displayName = userData.username || doc.id;
            option.textContent = displayName;
            userSelect.appendChild(option);
        });
        
        console.log(`Found ${usersList.length} users with role "user"`);
        
    } catch (error) {
        console.error("Error fetching users:", error);
        userSelect.innerHTML = '<option value="">Error loading users</option>';
    }
}

// Function to display user details when selected
function displayUserDetails(userId) {
    const user = usersList.find(u => u.id === userId);
    const userDetailsDiv = document.getElementById('user-details');
    const deleteMessage = document.getElementById('delete-message');
    
    if (!user) {
        userDetailsDiv.style.display = 'none';
        return;
    }
    
    currentSelectedUserId = userId;
    
    if (deleteMessage) {
        deleteMessage.textContent = '';
        deleteMessage.className = 'delete-message';
    }
    
    document.getElementById('user-doc-id').textContent = user.id;
    document.getElementById('user-username').textContent = user.username || 'Not provided';
    document.getElementById('user-email').textContent = user.email || 'Not provided';
    
    let createdDate = 'Not provided';
    if (user.createdAt) {
        if (user.createdAt.toDate) {
            createdDate = user.createdAt.toDate().toLocaleString();
        } else if (user.createdAt) {
            createdDate = new Date(user.createdAt).toLocaleString();
        }
    }
    document.getElementById('user-created').textContent = createdDate;
    
    userDetailsDiv.style.display = 'block';
}

// Function to delete a user
async function deleteUser(userId) {
    if (!userId) {
        console.error('No user selected');
        return;
    }
    
    const user = usersList.find(u => u.id === userId);
    const userName = user?.username || userId;
    
    const confirmDelete = confirm(`Are you sure you want to delete user "${userName}"? This action cannot be undone.`);
    
    if (!confirmDelete) {
        return;
    }
    
    const deleteMessage = document.getElementById('delete-message');
    deleteMessage.textContent = 'Deleting...';
    deleteMessage.className = 'delete-message';
    
    try {
        const userRef = doc(db, 'users', userId);
        await deleteDoc(userRef);
        
        console.log(`User ${userId} deleted successfully`);
        
        deleteMessage.textContent = '✓ User deleted successfully!';
        deleteMessage.style.color = '#28a745';
        
        usersList = usersList.filter(user => user.id !== userId);
        refreshUserDropdown();
        
        document.getElementById('user-details').style.display = 'none';
        currentSelectedUserId = null;
        
        setTimeout(() => {
            if (deleteMessage) {
                deleteMessage.textContent = '';
            }
        }, 3000);
        
    } catch (error) {
        console.error('Error deleting user:', error);
        deleteMessage.textContent = '✗ Error deleting user. Please try again.';
        deleteMessage.className = 'delete-message error';
        
        setTimeout(() => {
            if (deleteMessage) {
                deleteMessage.textContent = '';
                deleteMessage.className = 'delete-message';
            }
        }, 3000);
    }
}

// Function to refresh the user dropdown after deletion
function refreshUserDropdown() {
    const userSelect = document.getElementById('user-select');
    
    if (usersList.length === 0) {
        userSelect.innerHTML = '<option value="">No users found with role "user"</option>';
        return;
    }
    
    userSelect.innerHTML = '<option value="">Select a user...</option>';
    
    usersList.forEach(user => {
        const option = document.createElement('option');
        option.value = user.id;
        const displayName = user.username || user.id;
        option.textContent = displayName;
        userSelect.appendChild(option);
    });
}

// Function to fetch users for game data dropdown
async function fetchUsersForGameData() {
    const gameDataSelect = document.getElementById('game-data-user-select');
    gameDataSelect.innerHTML = '<option value="">Loading users...</option>';
    
    try {
        const usersRef = collection(db, 'users');
        const q = query(usersRef, where('role', '==', 'user'));
        const querySnapshot = await getDocs(q);
        
        if (querySnapshot.empty) {
            gameDataSelect.innerHTML = '<option value="">No users found</option>';
            return;
        }
        
        gameDataSelect.innerHTML = '<option value="">Select a user...</option>';
        
        const gameDataUsersList = [];
        querySnapshot.forEach((doc) => {
            const userData = doc.data();
            gameDataUsersList.push({
                id: doc.id,
                ...userData
            });
            
            const option = document.createElement('option');
            option.value = doc.id;
            const displayName = userData.username || doc.id;
            option.textContent = displayName;
            gameDataSelect.appendChild(option);
        });
        
        window.gameDataUsersList = gameDataUsersList;
        
    } catch (error) {
        console.error("Error fetching users for game data:", error);
        gameDataSelect.innerHTML = '<option value="">Error loading users</option>';
    }
}

// Function to display game data when user is selected
async function displayGameData(userId) {
    const gameDataDetailsDiv = document.getElementById('game-data-details');
    const gameProgressionDiv = document.getElementById('game-progression-content');
    const unityGameSaveDiv = document.getElementById('unity-game-save-content');
    
    if (!userId) {
        gameDataDetailsDiv.style.display = 'none';
        return;
    }
    
    const user = window.gameDataUsersList?.find(u => u.id === userId);
    
    if (!user) {
        gameDataDetailsDiv.style.display = 'none';
        return;
    }
    
    try {
        const userDocRef = doc(db, 'users', userId);
        const userDoc = await getDoc(userDocRef);
        
        if (!userDoc.exists()) {
            console.error('User document not found');
            gameProgressionDiv.innerHTML = '<div class="empty-state">User document not found</div>';
            unityGameSaveDiv.innerHTML = '<div class="empty-state">User document not found</div>';
            gameDataDetailsDiv.style.display = 'block';
            return;
        }
        
        const userData = userDoc.data();
        
        document.getElementById('game-data-username').textContent = user.username || user.id;
        
        // Game Progression Section
        let progressionHtml = '';
        const hasFinalTime = userData.finalTotalTime !== undefined && userData.finalTotalTime !== null;
        
        try {
            if (hasFinalTime) {
                const formattedTime = formatTime(userData.finalTotalTime);
                progressionHtml = `
                    <div class="nested-item">
                        <span class="nested-key">Status:</span>
                        <span class="nested-value" style="color: #28a745; font-weight: bold;">✓ Game Completed</span>
                    </div>
                    <div class="nested-item">
                        <span class="nested-key">Total Time:</span>
                        <span class="nested-value">${escapeHtml(formattedTime)}</span>
                    </div>
                `;
            } else {
                progressionHtml = `
                    <div class="nested-item">
                        <span class="nested-key">Status:</span>
                        <span class="nested-value" style="color: #ff9800; font-weight: bold;">⏳ In Progress</span>
                    </div>
                    <div class="nested-item">
                        <span class="nested-key">Total Time:</span>
                        <span class="nested-value">Not completed yet</span>
                    </div>
                `;
            }
        } catch (timeError) {
            console.error('Error formatting time:', timeError);
            progressionHtml = `
                <div class="nested-item">
                    <span class="nested-key">Status:</span>
                    <span class="nested-value" style="color: #ff9800; font-weight: bold;">⏳ In Progress</span>
                </div>
                <div class="nested-item">
                    <span class="nested-key">Total Time:</span>
                    <span class="nested-value">Data available but error formatting</span>
                </div>
            `;
        }
        
        gameProgressionDiv.innerHTML = progressionHtml;
        
        // Unity Game Save Data Section
        if (userData.unityGameSaveObject) {
            const unityGameSaveHtml = displayNestedObject(userData.unityGameSaveObject);
            unityGameSaveDiv.innerHTML = unityGameSaveHtml;
        } else {
            unityGameSaveDiv.innerHTML = '<div class="empty-state">No unityGameSaveObject found for this user</div>';
        }
        
        gameDataDetailsDiv.style.display = 'block';
        
    } catch (error) {
        console.error("Error fetching game data:", error);
        gameProgressionDiv.innerHTML = `<div class="empty-state error">Error loading game data: ${error.message}</div>`;
        unityGameSaveDiv.innerHTML = `<div class="empty-state error">Error loading game data: ${error.message}</div>`;
        gameDataDetailsDiv.style.display = 'block';
    }
}

// Function to fetch users with role "admin"
async function fetchAdmins() {
    const adminSelect = document.getElementById('admin-select');
    adminSelect.innerHTML = '<option value="">Loading admins...</option>';
    
    try {
        const usersRef = collection(db, 'users');
        const q = query(usersRef, where('role', '==', 'admin'));
        const querySnapshot = await getDocs(q);
        
        if (querySnapshot.empty) {
            adminSelect.innerHTML = '<option value="">No admins found</option>';
            console.log('No admins found');
            return;
        }
        
        adminSelect.innerHTML = '<option value="">Select an admin...</option>';
        
        adminsList = [];
        querySnapshot.forEach((doc) => {
            const adminData = doc.data();
            adminsList.push({
                id: doc.id,
                ...adminData
            });
            
            const option = document.createElement('option');
            option.value = doc.id;
            const displayName = adminData.username || doc.id;
            option.textContent = displayName;
            adminSelect.appendChild(option);
        });
        
        console.log(`Found ${adminsList.length} admins`);
        
    } catch (error) {
        console.error("Error fetching admins:", error);
        adminSelect.innerHTML = '<option value="">Error loading admins</option>';
    }
}

// Function to display admin details when selected
function displayAdminDetails(adminId) {
    const admin = adminsList.find(a => a.id === adminId);
    const adminDetailsDiv = document.getElementById('admin-details');
    const deleteMessage = document.getElementById('admin-delete-message');
    
    if (!admin) {
        adminDetailsDiv.style.display = 'none';
        return;
    }
    
    currentSelectedAdminId = adminId;
    
    if (deleteMessage) {
        deleteMessage.textContent = '';
        deleteMessage.className = 'delete-message';
    }
    
    document.getElementById('admin-doc-id').textContent = admin.id;
    document.getElementById('admin-username').textContent = admin.username || 'Not provided';
    document.getElementById('admin-email').textContent = admin.email || 'Not provided';
    
    let createdDate = 'Not provided';
    if (admin.createdAt) {
        if (admin.createdAt.toDate) {
            createdDate = admin.createdAt.toDate().toLocaleString();
        } else if (admin.createdAt) {
            createdDate = new Date(admin.createdAt).toLocaleString();
        }
    }
    document.getElementById('admin-created').textContent = createdDate;
    
    adminDetailsDiv.style.display = 'block';
}

// Function to delete an admin
async function deleteAdmin(adminId) {
    if (!adminId) {
        console.error('No admin selected');
        return;
    }
    
    const admin = adminsList.find(a => a.id === adminId);
    const adminName = admin?.username || adminId;
    
    const confirmDelete = confirm(`Are you sure you want to delete admin "${adminName}"? This action cannot be undone.`);
    
    if (!confirmDelete) {
        return;
    }
    
    const deleteMessage = document.getElementById('admin-delete-message');
    deleteMessage.textContent = 'Deleting...';
    deleteMessage.className = 'delete-message';
    
    try {
        const adminRef = doc(db, 'users', adminId);
        await deleteDoc(adminRef);
        
        console.log(`Admin ${adminId} deleted successfully`);
        
        deleteMessage.textContent = '✓ Admin deleted successfully!';
        deleteMessage.style.color = '#28a745';
        
        adminsList = adminsList.filter(admin => admin.id !== adminId);
        refreshAdminDropdown();
        
        document.getElementById('admin-details').style.display = 'none';
        currentSelectedAdminId = null;
        
        setTimeout(() => {
            if (deleteMessage) {
                deleteMessage.textContent = '';
            }
        }, 3000);
        
    } catch (error) {
        console.error('Error deleting admin:', error);
        deleteMessage.textContent = '✗ Error deleting admin. Please try again.';
        deleteMessage.className = 'delete-message error';
        
        setTimeout(() => {
            if (deleteMessage) {
                deleteMessage.textContent = '';
                deleteMessage.className = 'delete-message';
            }
        }, 3000);
    }
}

// Function to refresh the admin dropdown after deletion
function refreshAdminDropdown() {
    const adminSelect = document.getElementById('admin-select');
    
    if (adminsList.length === 0) {
        adminSelect.innerHTML = '<option value="">No admins found</option>';
        return;
    }
    
    adminSelect.innerHTML = '<option value="">Select an admin...</option>';
    
    adminsList.forEach(admin => {
        const option = document.createElement('option');
        option.value = admin.id;
        const displayName = admin.username || admin.id;
        option.textContent = displayName;
        adminSelect.appendChild(option);
    });
}

// Function to add a new admin (creates Auth user + Firestore document)
async function addAdmin(username, email, password) {
    try {
        // Step 1: Create the user in Firebase Authentication
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);
        const authUser = userCredential.user;
        
        console.log(`Auth user created with UID: ${authUser.uid}`);
        
        // Step 2: Create the admin document in Firestore (without password)
        const usersRef = collection(db, 'users');
        const newAdminRef = doc(usersRef, authUser.uid); // Use Auth UID as document ID
        
        const adminData = {
            username: username,
            email: email,
            role: 'admin',
            createdAt: new Date().toISOString(),
            authUID: authUser.uid
        };
        
        await setDoc(newAdminRef, adminData);
        
        console.log(`Admin document created with ID: ${newAdminRef.id}`);
        return { success: true, id: newAdminRef.id, uid: authUser.uid };
        
    } catch (error) {
        console.error("Error adding admin:", error);
        
        let errorMessage = error.message;
        if (error.code === 'auth/email-already-in-use') {
            errorMessage = 'This email is already registered';
        } else if (error.code === 'auth/weak-password') {
            errorMessage = 'Password should be at least 6 characters';
        } else if (error.code === 'auth/invalid-email') {
            errorMessage = 'Invalid email format';
        }
        
        return { success: false, error: errorMessage };
    }
}

// Function to show the add admin modal
function showAddAdminModal() {
    const modal = document.getElementById('add-admin-modal');
    const usernameInput = document.getElementById('new-admin-username');
    const emailInput = document.getElementById('new-admin-email');
    const passwordInput = document.getElementById('new-admin-password');
    const modalMessage = document.getElementById('modal-message');
    
    // Clear previous values and messages
    usernameInput.value = '';
    emailInput.value = '';
    passwordInput.value = '';
    modalMessage.textContent = '';
    modalMessage.className = 'modal-message';
    
    // Show modal
    modal.style.display = 'flex';
}

// Function to hide the add admin modal
function hideAddAdminModal() {
    const modal = document.getElementById('add-admin-modal');
    modal.style.display = 'none';
}

// Function to handle the add admin form submission
async function handleAddAdmin() {
    const usernameInput = document.getElementById('new-admin-username');
    const emailInput = document.getElementById('new-admin-email');
    const passwordInput = document.getElementById('new-admin-password');
    const modalMessage = document.getElementById('modal-message');
    
    const username = usernameInput.value.trim();
    const email = emailInput.value.trim();
    const password = passwordInput.value.trim();
    
    // Validate inputs
    if (!username) {
        modalMessage.textContent = 'Please enter a username';
        modalMessage.className = 'modal-message error';
        return;
    }
    
    if (!email) {
        modalMessage.textContent = 'Please enter an email';
        modalMessage.className = 'modal-message error';
        return;
    }
    
    if (!password) {
        modalMessage.textContent = 'Please enter a password';
        modalMessage.className = 'modal-message error';
        return;
    }
    
    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        modalMessage.textContent = 'Please enter a valid email address';
        modalMessage.className = 'modal-message error';
        return;
    }
    
    // Disable confirm button while processing
    const confirmBtn = document.getElementById('modal-confirm-btn');
    confirmBtn.disabled = true;
    confirmBtn.textContent = 'Creating...';
    modalMessage.textContent = '';
    
    try {
        const result = await addAdmin(username, email, password);
        
        if (result.success) {
            modalMessage.textContent = '✓ Admin created successfully!';
            modalMessage.className = 'modal-message success';
            
            // Refresh the admin list
            await fetchAdmins();
            
            // Close modal after a short delay
            setTimeout(() => {
                hideAddAdminModal();
                confirmBtn.disabled = false;
                confirmBtn.textContent = 'Create Admin';
            }, 1500);
        } else {
            modalMessage.textContent = `✗ Error: ${result.error}`;
            modalMessage.className = 'modal-message error';
            confirmBtn.disabled = false;
            confirmBtn.textContent = 'Create Admin';
        }
        
    } catch (error) {
        modalMessage.textContent = `✗ Error: ${error.message}`;
        modalMessage.className = 'modal-message error';
        confirmBtn.disabled = false;
        confirmBtn.textContent = 'Create Admin';
    }
}

// Function to switch between content sections
function switchTab(tabId) {
    const sections = {
        'tab-user-settings': 'content-user-settings',
        'tab-user-game-data': 'content-user-game-data',
        'tab-admin-settings': 'content-admin-settings'
    };
    
    // Hide all content sections
    document.getElementById('content-user-settings').style.display = 'none';
    document.getElementById('content-user-game-data').style.display = 'none';
    document.getElementById('content-admin-settings').style.display = 'none';
    
    // Show the selected content section
    const contentId = sections[tabId];
    if (contentId) {
        document.getElementById(contentId).style.display = 'block';
    }
    
    // Remove active class from all tabs
    const tabs = ['tab-user-settings', 'tab-user-game-data', 'tab-admin-settings'];
    tabs.forEach(tab => {
        const element = document.getElementById(tab);
        if (element) {
            element.classList.remove('active');
        }
    });
    
    // Add active class to the clicked tab
    const clickedTab = document.getElementById(tabId);
    if (clickedTab) {
        clickedTab.classList.add('active');
    }
    
    // Load data based on which tab is selected
    if (tabId === 'tab-user-settings') {
        fetchUsers();
    } else if (tabId === 'tab-user-game-data') {
        fetchUsersForGameData();
    } else if (tabId === 'tab-admin-settings') {
        fetchAdmins();
    }
}

document.addEventListener('DOMContentLoaded', () => {
    // Tab click listeners
    const userSettingsTab = document.getElementById('tab-user-settings');
    const userGameDataTab = document.getElementById('tab-user-game-data');
    const adminSettingsTab = document.getElementById('tab-admin-settings');
    
    if (userSettingsTab) {
        userSettingsTab.addEventListener('click', () => switchTab('tab-user-settings'));
    }
    
    if (userGameDataTab) {
        userGameDataTab.addEventListener('click', () => switchTab('tab-user-game-data'));
    }
    
    if (adminSettingsTab) {
        adminSettingsTab.addEventListener('click', () => switchTab('tab-admin-settings'));
    }
    
    // Set default active tab
    switchTab('tab-user-settings');
    
    // User Settings dropdown
    const userSelect = document.getElementById('user-select');
    if (userSelect) {
        userSelect.addEventListener('change', (event) => {
            if (event.target.value) {
                displayUserDetails(event.target.value);
            } else {
                document.getElementById('user-details').style.display = 'none';
                currentSelectedUserId = null;
            }
        });
    }
    
    // User Settings delete button
    const deleteBtn = document.getElementById('delete-user-btn');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', () => {
            if (currentSelectedUserId) {
                deleteUser(currentSelectedUserId);
            } else {
                alert('Please select a user first');
            }
        });
    }
    
    // User Game Data dropdown
    const gameDataSelect = document.getElementById('game-data-user-select');
    if (gameDataSelect) {
        gameDataSelect.addEventListener('change', (event) => {
            if (event.target.value) {
                displayGameData(event.target.value);
            } else {
                document.getElementById('game-data-details').style.display = 'none';
            }
        });
    }
    
    // Admin Settings dropdown
    const adminSelect = document.getElementById('admin-select');
    if (adminSelect) {
        adminSelect.addEventListener('change', (event) => {
            if (event.target.value) {
                displayAdminDetails(event.target.value);
            } else {
                document.getElementById('admin-details').style.display = 'none';
                currentSelectedAdminId = null;
            }
        });
    }
    
    // Admin Settings delete button
    const deleteAdminBtn = document.getElementById('delete-admin-btn');
    if (deleteAdminBtn) {
        deleteAdminBtn.addEventListener('click', () => {
            if (currentSelectedAdminId) {
                deleteAdmin(currentSelectedAdminId);
            } else {
                alert('Please select an admin first');
            }
        });
    }
    
    // Add Admin button click
    const addAdminBtn = document.getElementById('add-admin-btn');
    if (addAdminBtn) {
        addAdminBtn.addEventListener('click', showAddAdminModal);
    }
    
    // Modal close button
    const modalClose = document.querySelector('.modal-close');
    if (modalClose) {
        modalClose.addEventListener('click', hideAddAdminModal);
    }
    
    // Modal cancel button
    const modalCancel = document.getElementById('modal-cancel-btn');
    if (modalCancel) {
        modalCancel.addEventListener('click', hideAddAdminModal);
    }
    
    // Modal confirm button
    const modalConfirm = document.getElementById('modal-confirm-btn');
    if (modalConfirm) {
        modalConfirm.addEventListener('click', handleAddAdmin);
    }
    
    // Close modal when clicking outside
    window.addEventListener('click', (event) => {
        const modal = document.getElementById('add-admin-modal');
        if (event.target === modal) {
            hideAddAdminModal();
        }
    });
});