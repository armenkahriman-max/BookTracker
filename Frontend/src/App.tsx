import { Link, Route, Routes } from "react-router-dom";
import { AccountPage } from "./auth/AccountPage";
import { LoginPage } from "./auth/LoginPage";
import { LogoutButton } from "./auth/LogoutButton";
import { BookListPage } from "./books/BookListPage";

function HomePage() {
  return <h1>Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <nav>
        <Link to="/">Home</Link>{" "}
        <Link to="/login">Log in</Link>{" "}
        <Link to="/account">Account</Link>{" "}
        <Link to="/books">Books</Link>{" "}
        <LogoutButton />
      </nav>

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/books" element={<BookListPage />} />
      </Routes>
    </>
  );
}