//PreparedStatement works by sending the SQL command structure to the database first, this parses the syntax, checks the table names,
//and creates an Execution Plan before it ever sees the user's input.. When the user data is finally sent, the database has already finished its execution plan
// and treats that data strictly as a Literal Value, not as code. Because the command and the data are handled in separate steps,
// any malicious input—like OR 1=1—is forced to stay as a harmless text string that cannot change how the database behaves.



// Logic: Build the command and data together as one string
String sql = "SELECT * FROM users WHERE user='" + user + "' AND pass='" + pass + "'";
Statement st = conn.createStatement();
ResultSet rs = st.executeQuery(sql);

// Logic: Send the template (?) first, then bind the data separately
String sql = "SELECT * FROM users WHERE user = ? AND pass = ?";
PreparedStatement st = conn.prepareStatement(sql);
st.setString(1, user);
st.setString(2, pass);
ResultSet rs = st.executeQuery();





//In the secure version, the developer separates the HTML structure from the user input so the browser knows exactly what is a
//command and what is just data. By using textContent, you are explicitly telling the browser to treat the input as a "Text Node,"
//which is a data type that cannot be executed as code. Even if a hacker submits a <script> tag, the browser will ignore the tags
//and only display the literal characters on the screen.


let comment = request.body.comment;
display.innerHTML = "<div class='message'>" + comment + "</div>";


let comment = request.body.comment;
display.innerHTML = "<div class='message'></div>";
display.querySelector(".message").textContent = comment;



//The browser automatically attaches cookies to every request made to a website, regardless of where the request originated,
//which allows hackers to "piggyback" on your active session. In contrast, a CSRF token must be manually included in the
//request body by the website's own code. Since security policies prevent a malicious site from reading the HTML of another site,
//the hacker has no way to retrieve or guess the secret token. Therefore, while a cookie proves you are logged in, the CSRF token
///proves the request actually came from the official bank website



<form action="/transfer" method="POST">
  <input type="text" name="amount">
  <input type="text" name="to\_account">
  <button type="submit">Transfer</button>
</form>





<form action="/transfer" method="POST">
  <input type="hidden" name="csrf\_token" value="${session.csrf\_token}">
  <input type="text" name="amount">
  <input type="text" name="to\_account">
  <button type="submit">Transfer</button>
</form>



// Only checks if the user is logged in
app.get("/admin/dashboard", authenticate, (req, res) => {
  res.render("admin_panel");
});


// Checks if the user is logged in AND has the 'admin' role
app.get("/admin/dashboard", authenticate, requireRole("admin"), (req, res) => {
  res.render("admin_panel");
});





// JWT EXPLOITATION

const token = req.headers['auth'];
const data = jwt.decode(token);

if (data.membership === "premium") {
  grantAccess();
}

const token = req.headers['auth'];
const data = jwt.verify(token, "MY_SECRET_KEY");

if (data.membership === "premium") {
  grantAccess();
}