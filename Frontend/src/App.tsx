import { Link, Route, Routes } from "react-router-dom";

function HomePage() {
  return <h1>Book Tracker</h1>;
}

function AboutPage() {
  return <h1>About Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <nav>
        <Link to="/">Home</Link>{" "}
        <Link to="/about">About</Link>
      </nav>

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/about" element={<AboutPage />} />
      </Routes>
    </>
  );
}