const express = require('express');
const app = express();
const port = 3000;

// Import node-fetch for compatibility with Node.js versions < 18
// If you are running Node.js v18 or newer, you can remove this line
// as 'fetch' is globally available.
const fetch = require('node-fetch');

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// --- API Helper Functions ---

/**
 * Handles fetching from microservices, parsing responses, and robust error handling.
 * @param {string} url - The URL of the microservice endpoint.
 * @param {object} options - Fetch options (method, headers, body).
 * @param {string} responseType - Expected response type ('json' or 'text').
 * @param {string} serviceName - Name of the service for error logging.
 */
async function callMicroservice(url, options, responseType = 'json', serviceName = 'Service') {
    try {
        const res = await fetch(url, options);

        if (!res.ok) {
            let errorBody;
            try {
                // Try parsing as JSON first for more structured errors
                errorBody = await res.json();
            } catch (jsonError) {
                // Fallback to text if JSON parsing fails
                errorBody = await res.text();
            }
            const errorMessage = `HTTP error! Status: ${res.status} - ${res.statusText}. Response: ${JSON.stringify(errorBody)}`;
            throw new Error(`${serviceName} failed: ${errorMessage}`);
        }

        if (responseType === 'json') {
            return await res.json();
        } else {
            return await res.text();
        }
    } catch (e) {
        // Re-throw with more context, ensuring it's an Error object
        if (e instanceof Error) {
            throw new Error(`${serviceName} call failed: ${e.message}`);
        } else {
            throw new Error(`${serviceName} call failed: ${String(e)}`);
        }
    }
}

// User Management APIs
async function signUp(username, password) {
    return callMicroservice(
        'http://localhost:5062/api/UserManagementApi/SignUp',
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userName: username, password })
        },
        'text', // Expects User ID as plain text
        'SignUp'
    );
}

async function login(username, password) {
    return callMicroservice(
        'http://localhost:5062/api/UserManagementApi/Login',
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userName: username, password })
        },
        'text', // Expects User ID as plain text
        'Login'
    );
}

// TransactionService API
async function transfer(senderId, receiverId, amount) {
    return callMicroservice(
        'http://localhost:5063/api/transactionsApis',
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ senderId, receiverId, moneyValue: amount })
        },
        'text', // Expects Transaction ID as plain text
        'Transfer'
    );
}

// ReportingService APIs
async function getTotalMoneySent(userId) {
    const url = `http://localhost:5064/api/ReportingAPI/${encodeURIComponent('Total money sent')}?id=${userId}`;
    return callMicroservice(
        url,
        {},
        'text', // Expects total amount as plain text
        'Get Total Money Sent'
    );
}

async function getTotalMoneyReceived(userId) {
    const url = `http://localhost:5064/api/ReportingAPI/${encodeURIComponent('Total money received')}?id=${userId}`;
    return callMicroservice(
        url,
        {},
        'text', // Expects total amount as plain text
        'Get Total Money Received'
    );
}

async function getAccountsSentMoneyToMe(userId) {
    const url = `http://localhost:5064/api/ReportingAPI/${encodeURIComponent('Accounts sent money to me')}?userId=${userId}`;
    return callMicroservice(
        url,
        {},
        'json', // Expects JSON array
        'Get Accounts Sent Money To Me'
    );
}

async function getAccountsReceivedMoneyFromMe(userId) {
    const url = `http://localhost:5064/api/ReportingAPI/${encodeURIComponent('Accounts received money from Me')}?userId=${userId}`;
    return callMicroservice(
        url,
        {},
        'json', // Expects JSON array
        'Get Accounts Received Money From Me'
    );
}

async function getUserLogs(userId) {
    const url = `http://localhost:5063/api/transactionsApis/GetLogs/${userId}`;
    return callMicroservice(
        url,
        {},
        'json', // Expects JSON object with sent and received transactions
        'Get User Logs'
    );
}

// --- Frontend Serving and HTML Generation ---

// Serve static assets (if you have a 'public' folder for CSS, JS, images, etc.)
app.use(express.static('public'));

// Main application HTML page
app.get('/', (req, res) => {
    res.send(`
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>InstaPay</title>
      <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
      <style>
        :root {
          --primary-color: #0050cc;
          --secondary-color: #0050cc;
          --accent-color: #0050cc;
          --success-color: #00c851;
          --danger-color: #ff3547;
          --light-gray: #f8f9fa;
          --medium-gray: #e9ecef;
          --dark-gray: #343a40;
          --shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          --radius: 8px;
        }

        * {
          margin: 0;
          padding: 0;
          box-sizing: border-box;
          font-family: 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
        }

        body {
          background-color: #f5f7fa;
          color: #333;
        }

        .container {
          max-width: 1200px;
          margin: 0 auto;
          padding: 0 20px;
        }

        header {
          background-color: white;
          box-shadow: var(--shadow);
          position: sticky;
          top: 0;
          z-index: 100;
        }

        .navbar {
          display: flex;
          justify-content: flex-end;
          align-items: left;
          padding: 15px 0;
        }

        .logo {
          margin-left: auto;
          display: flex;
          align-items: center;
          font-weight: bold;
          font-size: 24px;
          color: var(--primary-color); /* Changed to primary color for visibility */
          text-decoration: none; /* Added for anchor tag */
        }

        .logo i {
          margin-right: 10px;
          font-size: 28px;
        }

        main {
          padding: 30px 0;
        }

        .card {
          background: white;
          border-radius: var(--radius);
          box-shadow: var(--shadow);
          margin-bottom: 25px;
          overflow: hidden;
        }

        .card-header {
          padding: 15px 20px;
          background-color: var(--primary-color);
          color: white;
          font-size: 18px;
          font-weight: 600;
          display: flex;
          align-items: center;
        }

        .card-header i {
          margin-right: 10px;
        }

        .card-body {
          padding: 20px;
        }

        .tabs {
          display: flex;
          border-bottom: 1px solid var(--medium-gray);
          margin-bottom: 20px;
        }

        .tab-item {
          padding: 10px 20px;
          cursor: pointer;
          border-bottom: 3px solid transparent;
          transition: all 0.3s ease;
        }

        .tab-item.active {
          color: var(--primary-color);
          border-bottom-color: var(--primary-color);
          font-weight: 600;
        }

        .tab-content {
          display: none;
        }

        .tab-content.active {
          display: block;
        }

        .form-group {
          margin-bottom: 15px;
        }

        .form-label {
          display: block;
          margin-bottom: 8px;
          font-weight: 500;
          color: #555;
        }

        .form-control {
          width: 100%;
          padding: 12px 15px;
          border: 1px solid #ced4da;
          border-radius: var(--radius);
          font-size: 16px;
          transition: border-color 0.3s;
        }

        .form-control:focus {
          outline: none;
          border-color: var(--primary-color);
          box-shadow: 0 0 0 3px rgba(0, 102, 255, 0.1);
        }

        .form-select {
          width: 100%;
          padding: 12px 15px;
          border: 1px solid #ced4da;
          border-radius: var(--radius);
          font-size: 16px;
          appearance: none;
          background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='%23333' viewBox='0 0 16 16'%3E%3Cpath d='M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z'/%3E%3C/svg%3E");
          background-repeat: no-repeat;
          background-position: right 12px center;
          background-size: 16px;
        }

        .btn {
          display: block;
          width: 100%;
          padding: 12px 20px;
          background-color: var(--primary-color);
          color: white;
          border: none;
          border-radius: var(--radius);
          font-size: 16px;
          font-weight: 600;
          cursor: pointer;
          transition: background-color 0.2s;
          text-align: center;
        }

        .btn:hover {
          background-color: var(--secondary-color);
        }

        .btn-accent {
          background-color: var(--accent-color);
        }

        .btn-accent:hover {
          background-color: #0050cc;
        }

        .transfer-card {
          background: linear-gradient(135deg, var(--primary-color), var(--secondary-color));
          color: white;
          padding: 25px;
          border-radius: var(--radius);
          box-shadow: var(--shadow);
          margin-bottom: 25px;
        }

        .transfer-title {
          font-size: 24px;
          margin-bottom: 15px;
          font-weight: 600;
        }

        .transfer-icon {
          font-size: 48px;
          margin-bottom: 15px;
          text-align: center;
        }

        .dashboard-stats {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
          gap: 20px;
          margin-bottom: 30px;
        }

        .stat-card {
          background: white;
          border-radius: var(--radius);
          box-shadow: var(--shadow);
          padding: 20px;
          text-align: center;
        }

        .stat-icon {
          font-size: 32px;
          margin-bottom: 10px;
          color: var(--primary-color);
        }

        .stat-value {
          font-size: 24px;
          font-weight: bold;
          margin-bottom: 5px;
        }

        .stat-label {
          color: #666;
          font-size: 14px;
        }

        .transaction-list {
          list-style: none;
        }

        .transaction-item {
          display: flex;
          align-items: center;
          padding: 15px 0;
          border-bottom: 1px solid var(--medium-gray);
        }

        .transaction-icon {
          width: 40px;
          height: 40px;
          border-radius: 50%;
          background-color: var(--primary-color);
          color: white;
          display: flex;
          align-items: center;
          justify-content: center;
          margin-right: 15px;
        }

        .transaction-details {
          flex-grow: 1;
        }

        .transaction-title {
          font-weight: 500;
          margin-bottom: 3px;
        }

        .transaction-date {
          font-size: 12px;
          color: #666;
        }

        .transaction-amount {
          font-weight: 600;
        }

        .transaction-amount.sent {
          color: var(--danger-color);
        }

        .transaction-amount.received {
          color: var(--success-color);
        }

        .alert {
          padding: 15px;
          border-radius: var(--radius);
          margin-bottom: 20px;
          background-color: #fff3cd;
          border-left: 4px solid var(--accent-color);
          color: #856404;
        }

        .alert-success {
          background-color: #d4edda;
          border-left-color: var(--success-color);
          color: #155724;
        }

        .alert-danger {
          background-color: #f8d7da;
          border-left-color: var(--danger-color);
          color: #721c24;
        }

        /* Results styling */
        .result-container {
          background: white;
          padding: 20px;
          border-radius: var(--radius);
          box-shadow: var(--shadow);
          margin: 30px 0;
        }

        .result-header {
          padding-bottom: 15px;
          margin-bottom: 15px;
          border-bottom: 1px solid var(--medium-gray);
          font-size: 20px;
          color: var(--primary-color);
          display: flex;
          align-items: center;
        }

        .result-header i {
          margin-right: 10px;
        }

        .result-content {
          line-height: 1.6;
        }

        .result-value {
          font-size: 24px;
          font-weight: bold;
          margin: 10px 0;
          color: var(--dark-gray);
        }

        pre {
          background-color: var(--light-gray);
          padding: 15px;
          border-radius: var(--radius);
          overflow-x: auto;
          font-family: 'Courier New', monospace;
        }

        .back-link {
          display: inline-block;
          margin-top: 20px;
          color: var(--primary-color);
          text-decoration: none;
          font-weight: 500;
        }

        .back-link:hover {
          text-decoration: underline;
        }

        footer {
          background-color: var(--dark-gray);
          color: white;
          text-align: center;
          padding: 20px 0;
          margin-top: 50px;
        }

        /* Responsive design */
        @media (max-width: 768px) {
          .dashboard-stats {
            grid-template-columns: 1fr;
          }

          .card-body {
            padding: 15px;
          }
        }
      </style>
    </head>
    <body>
      <header>
        <div class="container">
          <div class="navbar">
            <a href="/" class="logo">
              <i class="fas fa-bolt"></i>
              InstaPay
            </a>
          </div>
        </div>
      </header>

      <main class="container">
        <section id="auth" class="card">
          <div class="card-header">
            <i class="fas fa-user-circle"></i>
            Account Access
          </div>
          <div class="card-body">
            <div class="tabs">
              <div class="tab-item active" data-tab="login">Login</div>
              <div class="tab-item" data-tab="signup">Sign Up</div>
            </div>

            <div class="tab-content active" id="login-tab">
              <form method="POST" action="/login">
                <div class="form-group">
                  <label class="form-label">Username</label>
                  <input type="text" name="username" class="form-control" required>
                </div>
                <div class="form-group">
                  <label class="form-label">Password</label>
                  <input type="password" name="password" class="form-control" required>
                </div>
                <button type="submit" class="btn">
                  <i class="fas fa-sign-in-alt"></i> Login
                </button>
              </form>
            </div>

            <div class="tab-content" id="signup-tab">
              <form method="POST" action="/signup">
                <div class="form-group">
                  <label class="form-label">Create Username</label>
                  <input type="text" name="username" class="form-control" required>
                </div>
                <div class="form-group">
                  <label class="form-label">Create Password</label>
                  <input type="password" name="password" class="form-control" required>
                </div>
                <button type="submit" class="btn">
                  <i class="fas fa-user-plus"></i> Create Account
                </button>
              </form>
            </div>
          </div>
        </section>

        <section id="transfer" class="card">
          <div class="card-header">
            <i class="fas fa-exchange-alt"></i>
            Money Transfer
          </div>
          <div class="card-body">
            <div class="transfer-card">
              <div class="transfer-icon">
                <i class="fas fa-paper-plane"></i>
              </div>
              <h2 class="transfer-title">Send Money Instantly</h2>
            </div>

            <form method="POST" action="/transfer">
              <div class="form-group">
                <label class="form-label">Your ID</label>
                <input type="number" name="senderId" class="form-control" required>
              </div>
              <div class="form-group">
                <label class="form-label">Recipient ID</label>
                <input type="number" name="receiverId" class="form-control" required>
              </div>
              <div class="form-group">
                <label class="form-label">Amount</label>
                <input type="number" name="amount" step="0.01" class="form-control" required>
              </div>
              <button type="submit" class="btn btn-accent">
                <i class="fas fa-paper-plane"></i> Send Money
              </button>
            </form>
          </div>
        </section>

        <section id="reporting" class="card">
          <div class="card-header">
            <i class="fas fa-chart-line"></i>
            Financial Reports
          </div>
          <div class="card-body">
            <form method="POST" action="/report">
              <div class="form-group">
                <label class="form-label">Your ID</label>
                <input type="number" name="userId" class="form-control" required>
              </div>
              <div class="form-group">
                <label class="form-label">Report Type</label>
                <select name="reportType" class="form-select" required>
                  <option value="totalSent">Total Money Sent</option>
                  <option value="totalReceived">Total Money Received</option>
                  <option value="accountsSentToMe">Accounts That Sent Me Money</option>
                  <option value="accountsReceivedFromMe">Accounts That Received Money From Me</option>
                  <option value="userLogs">Transaction History (Logs)</option>
                </select>
              </div>
              <button type="submit" class="btn">
                <i class="fas fa-file-alt"></i> Generate Report
              </button>
            </form>
          </div>
        </section>
      </main>

      <footer>
        <div class="container">
          <p>&copy; 2025 InstaPay. All rights reserved.</p>
        </div>
      </footer>

      <script>
        // Tab functionality
        document.querySelectorAll('.tab-item').forEach(tab => {
          tab.addEventListener('click', () => {
            // Remove active class from all tabs and content
            document.querySelectorAll('.tab-item').forEach(item => item.classList.remove('active'));
            document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));

            // Add active class to clicked tab
            tab.classList.add('active');

            // Show corresponding content
            const tabId = tab.getAttribute('data-tab');
            document.getElementById(tabId + '-tab').classList.add('active');
          });
        });
      </script>
    </body>
    </html>
    `);
});

// --- Result Page Styling Function ---
function styleResultPage(title, content) {
    return `
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, initial-scale=1.0">
      <title>InstaPay - ${title}</title>
      <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
      <style>
        :root {
        --accent-color: #007bff;
          --primary-color: #0050cc;
          --secondary-color: #0050cc;
          --accent-color: #0050cc;
          --success-color: #00c851;
          --danger-color: #ff3547;
          --light-gray: #f8f9fa;
          --medium-gray: #e9ecef;
          --dark-gray: #343a40;
          --shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          --radius: 8px;
        }

        * {
          margin: 0;
          padding: 0;
          box-sizing: border-box;
          font-family: 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
        }

        body {
          background-color: #f5f7fa;
          color: #333;
        }

        .container {
          max-width: 800px;
          margin: 0 auto;
          padding: 20px;
        }

        header {
          display: flex;
          align-items: center;
          margin-bottom: 30px;
        }

        .logo {
          display: flex;
          align-items: center;
          font-weight: bold;
          font-size: 24px;
          color: var(--primary-color);
          text-decoration: none;
        }

        .logo i {
          margin-right: 10px;
          font-size: 28px;
        }

        .result-container {
          background: white;
          padding: 25px;
          border-radius: var(--radius);
          box-shadow: var(--shadow);
        }

        .result-header {
          padding-bottom: 15px;
          margin-bottom: 20px;
          border-bottom: 1px solid var(--medium-gray);
          font-size: 22px;
          color: var(--primary-color);
          display: flex;
          align-items: center;
        }

        .result-header i {
          margin-right: 12px;
        }

        .result-content {
          line-height: 1.6;
        }

        .success-message {
          padding: 15px;
          background-color: #d4edda;
          border-left: 4px solid var(--success-color);
          color: #155724;
          border-radius: var(--radius);
          margin-bottom: 20px;
        }

        .error-message {
          padding: 15px;
          background-color: #f8d7da;
          border-left: 4px solid var(--danger-color);
          color: #721c24;
          border-radius: var(--radius);
          margin-bottom: 20px;
        }

        .result-value {
          font-size: 28px;
          font-weight: bold;
          margin: 15px 0;
          color: var(--dark-gray);
        }

        pre {
          background-color: var(--light-gray);
          padding: 15px;
          border-radius: var(--radius);
          overflow-x: auto;
          font-family: 'Courier New', monospace;
        }

        .back-link {
          display: inline-flex;
          align-items: center;
          margin-top: 25px;
          color: var(--primary-color);
          text-decoration: none;
          font-weight: 500;
          padding: 10px 15px;
          background-color: white;
          border-radius: var(--radius);
          box-shadow: var(--shadow);
          transition: transform 0.2s;
        }

        .back-link:hover {
          transform: translateY(-2px);
        }

        .back-link i {
          margin-right: 8px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }
        th, td {
            padding: 10px;
            border: 1px solid #ddd;
            text-align: left;
        }
        th {
            background-color: #f2f2f2;
        }
        .transaction-type-sent {
            color: var(--danger-color);
            font-weight: 500;
        }
        .transaction-type-received {
            color: var(--success-color);
            font-weight: 500;
        }
      </style>
    </head>
    <body>
      <div class="container">
        <header>
          <a href="/" class="logo">
            <i class="fas fa-bolt"></i>
            InstaPay
          </a>
        </header>

        <div class="result-container">
          <div class="result-header">
            <i class="fas fa-info-circle"></i>
            ${title}
          </div>
          <div class="result-content">
            ${content}
          </div>
          <a href="/" class="back-link">
            <i class="fas fa-arrow-left"></i> Back to Home
          </a>
        </div>
      </div>
    </body>
    </html>
    `;
}

// --- Express Route Handlers ---

app.post('/signup', async (req, res) => {
    try {
        const userId = await signUp(req.body.username, req.body.password);
        const content = `
            <div class="success-message">
                <i class="fas fa-check-circle"></i> Account created successfully!
            </div>
            <p>Your account has been registered. You can now login with your credentials.</p>
            <p>Your new User ID: <strong>${userId}</strong></p>
            <p>Please save your User ID for future transactions and reports.</p>
        `;
        res.send(styleResultPage('Sign Up Successful', content));
    } catch (e) {
        const content = `
            <div class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Sign Up failed
            </div>
            <p>${e.message}</p>
        `;
        res.status(400).send(styleResultPage('Sign Up Error', content));
    }
});

app.post('/login', async (req, res) => {
    try {
        const userId = await login(req.body.username, req.body.password);
        const content = `
            <div class="success-message">
                <i class="fas fa-check-circle"></i> Login successful!
            </div>
            <p>You have successfully logged into your account.</p>
            <p>Your User ID: <strong>${userId}</strong></p>
            <p>Please save your User ID for transactions and reports.</p>
        `;
        res.send(styleResultPage('Login Successful', content));
    } catch (e) {
        const content = `
            <div class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Login failed
            </div>
            <p>${e.message}</p>
        `;
        res.status(400).send(styleResultPage('Login Error', content));
    }
});

app.post('/transfer', async (req, res) => {
    const { senderId, receiverId, amount } = req.body;

    if (isNaN(senderId) || isNaN(receiverId) || isNaN(amount) || Number(amount) <= 0) {
        const content = `
            <div class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Invalid input
            </div>
            <p>Please provide valid numeric values for sender ID, recipient ID, and a positive amount.</p>
        `;
        return res.status(400).send(styleResultPage('Transfer Error', content));
    }

    try {
        const transactionId = await transfer(Number(senderId), Number(receiverId), Number(amount));
        const content = `
            <div class="success-message">
                <i class="fas fa-check-circle"></i> Money sent successfully!
            </div>
            <p>Your transaction has been processed.</p>
            <p>From User ID: <strong>${senderId}</strong></p>
            <p>To User ID: <strong>${receiverId}</strong></p>
            <p>Amount: <strong>$${parseFloat(amount).toFixed(2)}</strong></p>
            <p>Transaction ID: <strong>${transactionId}</strong></p>
        `;
        res.send(styleResultPage('Transfer Successful', content));
    } catch (e) {
        const content = `
            <div class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Transfer failed
            </div>
            <p>${e.message}</p>
        `;
        res.status(400).send(styleResultPage('Transfer Error', content));
    }
});

app.post('/report', async (req, res) => {
    const { userId, reportType } = req.body;
    if (isNaN(userId)) {
        const content = `
            <div class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Invalid User ID
            </div>
            <p>Please provide a valid numeric User ID.</p>
        `;
        return res.status(400).send(styleResultPage('Report Error', content));
    }

    let result;
    let content;
    let title;
    let iconClass = 'fas fa-info-circle'; // Default icon

    try {
        switch (reportType) {
            case 'totalSent':
                result = await getTotalMoneySent(Number(userId));
                title = 'Total Money Sent';
                iconClass = 'fas fa-paper-plane';
                content = `
                    <p>User ID: <strong>${userId}</strong></p>
                    <p>Total money you have sent:</p>
                    <div class="result-value">$${parseFloat(result || 0).toFixed(2)}</div>
                `;
                break;

            case 'totalReceived':
                result = await getTotalMoneyReceived(Number(userId));
                title = 'Total Money Received';
                iconClass = 'fas fa-hand-holding-usd';
                content = `
                    <p>User ID: <strong>${userId}</strong></p>
                    <p>Total money you have received:</p>
                    <div class="result-value">$${parseFloat(result || 0).toFixed(2)}</div>
                `;
                break;

            case 'accountsSentToMe':
                result = await getAccountsSentMoneyToMe(Number(userId));
                title = 'Accounts That Sent Money To Me';
                iconClass = 'fas fa-users';
                content = `
                    <p>User ID: <strong>${userId}</strong></p>
                    <p>The following accounts have sent money to you:</p>
                    ${result.length === 0 ? '<p>No accounts have sent you money yet.</p>' : `
                        <div style="margin: 20px 0;">
                            <table style="width: 100%; border-collapse: collapse;">
                                <thead>
                                    <tr style="background-color: var(--light-gray);">
                                        <th style="padding: 12px; text-align: left; border-bottom: 2px solid var(--medium-gray);">User ID</th>
                                        <th style="padding: 12px; text-align: right; border-bottom: 2px solid var(--medium-gray);">Total Amount</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    ${result.map(item => `
                                        <tr style="border-bottom: 1px solid var(--medium-gray);">
                                            <td style="padding: 12px; text-align: left;">User ${item.userId}</td>
                                            <td style="padding: 12px; text-align: right; color: var(--success-color); font-weight: 500;">$${parseFloat(item.amount || 0).toFixed(2)}</td>
                                        </tr>
                                    `).join('')}
                                </tbody>
                            </table>
                        </div>
                    `}
                `;
                break;

            case 'accountsReceivedFromMe':
                result = await getAccountsReceivedMoneyFromMe(Number(userId));
                title = 'Accounts That Received Money From Me';
                iconClass = 'fas fa-users';
                content = `
                    <p>User ID: <strong>${userId}</strong></p>
                    <p>The following accounts have received money from you:</p>
                    ${result.length === 0 ? '<p>You have not sent money to any accounts yet.</p>' : `
                        <div style="margin: 20px 0;">
                            <table style="width: 100%; border-collapse: collapse;">
                                <thead>
                                    <tr style="background-color: var(--light-gray);">
                                        <th style="padding: 12px; text-align: left; border-bottom: 2px solid var(--medium-gray);">User ID</th>
                                        <th style="padding: 12px; text-align: right; border-bottom: 2px solid var(--medium-gray);">Total Amount</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    ${result.map(item => `
                                        <tr style="border-bottom: 1px solid var(--medium-gray);">
                                            <td style="padding: 12px; text-align: left;">User ${item.userId}</td>
                                            <td style="padding: 12px; text-align: right; color: var(--danger-color); font-weight: 500;">$${parseFloat(item.amount || 0).toFixed(2)}</td>
                                        </tr>
                                    `).join('')}
                                </tbody>
                            </table>
                        </div>
                    `}
                `;
                break;

            case 'userLogs':
                result = await getUserLogs(Number(userId));
                title = 'Transaction History (Logs)';
                iconClass = 'fas fa-history';

                const sentTransactions = result.sentTransactions || [];
                const receivedTransactions = result.receivedTransactions || [];

                content = `
                    <p>User ID: <strong>${userId}</strong></p>

                    <h4>Sent Transactions:</h4>
                    ${sentTransactions.length === 0 ? '<p>No sent transactions found.</p>' : `
                        <div style="margin: 10px 0 20px 0;">
                            <table style="width: 100%; border-collapse: collapse;">
                                <thead>
                                    <tr style="background-color: var(--light-gray);">
                                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid var(--medium-gray);">Transaction ID</th>
                                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid var(--medium-gray);">Receiver ID</th>
                                        <th style="padding: 10px; text-align: right; border-bottom: 2px solid var(--medium-gray);">Amount</th>
                                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid var(--medium-gray);">Date</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    ${sentTransactions.map(t => `
                                        <tr style="border-bottom: 1px solid var(--medium-gray);">
                                            <td style="padding: 10px;">${t.transactionId}</td>
                                            <td style="padding: 10px;">${t.receiverId}</td>
                                            <td style="padding: 10px; text-align: right;" class="transaction-type-sent">-$${parseFloat(t.moneyValue || 0).toFixed(2)}</td>
                                            <td style="padding: 10px;">${new Date(t.timestamp).toLocaleString()}</td>
                                        </tr>
                                    `).join('')}
                                </tbody>
                            </table>
                        </div>
                    `}

                    <h4>Received Transactions:</h4>
                    ${receivedTransactions.length === 0 ? '<p>No received transactions found.</p>' : `
                        <div style="margin: 10px 0 20px 0;">
                            <table style="width: 100%; border-collapse: collapse;">
                                <thead>
                                    <tr style="background-color: var(--light-gray);">
                                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid var(--medium-gray);">Transaction ID</th>
                                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid var(--medium-gray);">Sender ID</th>
                                        <th style="padding: 10px; text-align: right; border-bottom: 2px solid var(--medium-gray);">Amount</th>
                                        <th style="padding: 10px; text-align: left; border-bottom: 2px solid var(--medium-gray);">Date</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    ${receivedTransactions.map(t => `
                                        <tr style="border-bottom: 1px solid var(--medium-gray);">
                                            <td style="padding: 10px;">${t.transactionId}</td>
                                            <td style="padding: 10px;">${t.senderId}</td>
                                            <td style="padding: 10px; text-align: right;" class="transaction-type-received">+$${parseFloat(t.moneyValue || 0).toFixed(2)}</td>
                                            <td style="padding: 10px;">${new Date(t.timestamp).toLocaleString()}</td>
                                        </tr>
                                    `).join('')}
                                </tbody>
                            </table>
                        </div>
                    `}
                `;
                break;

            default:
                throw new Error('Invalid report type selected.');
        }

        res.send(styleResultPage(title, content.replace('result-header">', `result-header"><i class="${iconClass}"></i>`)));

    } catch (e) {
        content = `
            <div class="error-message">
                <i class="fas fa-exclamation-triangle"></i> Failed to generate report
            </div>
            <p>${e.message}</p>
        `;
        res.status(400).send(styleResultPage('Report Error', content));
    }
});


app.listen(port, () => {
    console.log(`Gateway API listening at http://localhost:${port}`);
});