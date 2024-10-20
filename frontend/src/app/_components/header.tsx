import Image from "next/image";
import Link from "next/link";
export default function Header() {
  return (
    <header className="h-[68px] px-[58px] bg-[#002F3F] text-white flex items-center justify-between">
      <div className="flex items-center">
        <Image
          src="/logo.svg"
          alt="Commerzbank Logo"
          width={245}
          height={30}
        />
      </div>
      <nav className="flex gap-6">
        <Link href="#" className="border-b-2 border-white px-[24px] py-[12px]">
          Dashboard
        </Link>
        <Link href="#" className="text-[#9EB0B6] hover:border-b-2 hover:border-white px-[24px] py-[12px]">
          Transactions
        </Link>
        <Link href="#" className="text-[#9EB0B6] hover:border-b-2 hover:border-white px-[24px] py-[12px]">
          Accounts
        </Link>
        <Link href="#" className="text-[#9EB0B6] hover:border-b-2 hover:border-white px-[24px] py-[12px]">
            Reports
        </Link>
        <Link href="#" className="text-[#9EB0B6] hover:border-b-2 hover:border-white px-[24px] py-[12px]">
          My data
        </Link>
        <Link href="#" className="text-[#9EB0B6] hover:border-b-2 hover:border-white px-[24px] py-[12px]">
          Help
        </Link>
      </nav>
      <div className="flex items-center gap-2">
        <button className="p-2 rounded-full hover:bg-[#005f73]">
            <Image src="/icon-bell.svg" alt="Notifications" width={24} height={24} />
        </button>
        <button className="p-2 rounded-full hover:bg-[#005f73]">
            <Image src="/icon-group.svg" alt="Group" width={24} height={24} />
        </button>
      </div>
    </header>
  );
}
