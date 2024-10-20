import CurrentDate from '@/app/_components/current-date';
import { EditDashboardButton, NewDashboardButton } from '@/app/_components/buttons';

export default function GoodMorningBar() {
  return (
    <div className="flex items-center justify-between">
      <div className="flex gap-4 items-center">
        <span className="text-[#697579] font-regular text-[15px]">Good morning Martina Mustermaier</span>
        <CurrentDate />
      </div>
      <div className="flex gap-2 items-center">
        <NewDashboardButton />
        <EditDashboardButton />
      </div>
    </div>
  );
};
